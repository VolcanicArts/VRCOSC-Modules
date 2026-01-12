// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.VRChat;

namespace VRCOSC.Modules.KAT;

[ModuleTitle("KAT")]
[ModuleDescription("Support for sending text over parameters using KillFrenzy Avatar Text")]
[ModuleType(ModuleType.Integrations)]
[ModulePrefab("KillFrenzy Avatar Text", "https://github.com/killfrenzy96/KillFrenzyAvatarText")]
public class KATModule : Module
{
    private const string param_char_sync_name = "KAT_CharSync";
    private const char invalid_char = '?';
    private const int text_length = 128;
    private const int sync_params_max = 16;
    private const int pointer_clear = 255;
    private const int sync_params_test_char = 97;

    private int pointerCount;
    private int pointerIndexReSync;
    private int invalidCharValue;

    private string currentText = string.Empty;

    private int moduleTestStep;
    private TimeSpan updateDelay;
    private DateTime lastUpdate;

    private int syncParamCountPrev;
    private int syncParamCount;

    private readonly Dictionary<char, int> charToCode = [];
    private readonly char[] codeToChar = new char[256];

    public string? TargetText { get; set; }

    protected override void OnPreLoad()
    {
        CreateTextBox(KATSetting.SyncParams, "Sync Params", "The number of sync parameters", 4);
        CreateTextBox(KATSetting.LineLength, "Line Length", "The line length", 32);
        CreateTextBox(KATSetting.LineCount, "Line Count", "The line count", 4);
        CreateTextBox(KATSetting.UpdateDelay, "Update Delay", "The update delay in milliseconds", 250);

        RegisterParameter<bool>(KATParameter.Visible, "KAT_Visible", ParameterMode.ReadWrite, "Visible", "Whether KAT is visible");
        RegisterParameter<int>(KATParameter.Pointer, "KAT_Pointer", ParameterMode.ReadWrite, "Pointer", "The pointer for writing/reading. Do not change");
        RegisterParameter<int>(KATParameter.CharSync, $"{param_char_sync_name}*", ParameterMode.ReadWrite, "CharSync", "The sync parameters with wildcard. Do not change");
    }

    protected override Task<bool> OnModuleStart()
    {
        syncParamCountPrev = 4;
        syncParamCount = GetSettingValue<int>(KATSetting.SyncParams);
        updateDelay = TimeSpan.FromMilliseconds(GetSettingValue<int>(KATSetting.UpdateDelay));

        buildKeyTables(charToCode, codeToChar);
        invalidCharValue = charToCode.GetValueOrDefault(invalid_char, 0);

        pointerCount = syncParamCount <= 0 ? 0 : text_length / syncParamCount;

        SendParameter(KATParameter.Visible, true);
        SendParameter(KATParameter.Pointer, pointer_clear);

        for (var i = 0; i < syncParamCount; i++)
            SendParameter($"{param_char_sync_name}{i}", 0f);

        return Task.FromResult(true);
    }

    protected override Task OnModuleStop()
    {
        SendParameter(KATParameter.Visible, false);
        return Task.CompletedTask;
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case KATParameter.CharSync:
                if (moduleTestStep <= 0) break;
                if (!parameter.IsValueType<int>()) break;
                if (!parameter.IsWildcardType<int>(0)) break;

                syncParamCount = Math.Max(syncParamCount, parameter.GetWildcard<int>(0) + 1);
                break;
        }
    }

    protected override void OnAvatarChange(AvatarConfig? avatarConfig)
    {
        moduleTestStep = 1;
    }

    public void SetVisiblity(bool visible) => SendParameter(KATParameter.Visible, visible);

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void update()
    {
        if (lastUpdate + updateDelay > DateTime.Now) return;

        lastUpdate = DateTime.Now;

        var localText = TargetText ?? string.Empty;

        if (moduleTestStep > 0)
        {
            SendParameter(KATParameter.Pointer, pointer_clear);

            switch (moduleTestStep)
            {
                case 1:
                {
                    syncParamCount = 0;

                    for (var i = 0; i < sync_params_max; i++)
                        SendParameter($"{param_char_sync_name}{i}", 0f);

                    moduleTestStep = 2;
                    return;
                }

                case 2:
                {
                    var testFloat = codeToFloat(sync_params_test_char);

                    for (var i = 0; i < sync_params_max; i++)
                        SendParameter($"{param_char_sync_name}{i}", testFloat);

                    moduleTestStep = 3;
                    return;
                }

                case 3:
                {
                    for (var i = 0; i < sync_params_max; i++)
                        SendParameter($"{param_char_sync_name}{i}", 0f);

                    moduleTestStep = 4;
                    return;
                }

                case 4:
                {
                    if (syncParamCount == 0)
                        syncParamCount = syncParamCountPrev;
                    else
                        syncParamCountPrev = syncParamCount;

                    moduleTestStep = 0;
                    pointerCount = text_length / syncParamCount;

                    currentText = new string(' ', text_length);
                    break;
                }
            }
        }

        if (syncParamCount == 0) return;

        if (string.IsNullOrWhiteSpace(localText) || moduleTestStep == -1)
        {
            if (moduleTestStep == -1)
            {
                for (var i = 0; i < sync_params_max; i++)
                    SendParameter($"{param_char_sync_name}{i}", 0f);

                moduleTestStep = 0;
            }

            SendParameter(KATParameter.Pointer, pointer_clear);
            currentText = new string(' ', text_length);
            return;
        }

        SendParameter(KATParameter.Visible, true);

        localText = formatText(localText);
        var oscText = (currentText ?? string.Empty).PadRight(text_length);

        if (!string.Equals(localText, currentText, StringComparison.Ordinal))
        {
            for (var pointerIndex = 0; pointerIndex < pointerCount; pointerIndex++)
            {
                var equal = true;
                var baseIndex = pointerIndex * syncParamCount;

                for (var i = 0; i < syncParamCount; i++)
                {
                    var offsetIndex = baseIndex + i;
                    if (localText[offsetIndex] == oscText[offsetIndex]) continue;

                    equal = false;
                    break;
                }

                if (equal) continue;

                updatePointer(pointerIndex, localText);
                return;
            }
        }

        pointerIndexReSync++;

        if (pointerIndexReSync >= pointerCount)
            pointerIndexReSync = 0;

        updatePointer(pointerIndexReSync, localText);
    }

    private void updatePointer(int pointer, string text)
    {
        SendParameter(KATParameter.Pointer, pointer + 1);

        var oscChars = (currentText ?? string.Empty).PadRight(text_length).ToCharArray();
        var baseIndex = pointer * syncParamCount;

        for (var charIndex = 0; charIndex < syncParamCount; charIndex++)
        {
            var subIndex = baseIndex + charIndex;
            var ch = text[subIndex];

            var floatValue = codeToFloat(charToCode.GetValueOrDefault(ch, invalidCharValue));

            SendParameter($"{param_char_sync_name}{charIndex}", floatValue);
            oscChars[subIndex] = ch;
        }

        currentText = new string(oscChars);
    }

    private static float codeToFloat(int code)
    {
        var codeF = (float)code;
        return (codeF > 127.5f ? codeF - 256f : codeF) / 127f;
    }

    private string formatText(string text)
    {
        var lineLength = GetSettingValue<int>(KATSetting.LineLength);

        var combined = string.Concat(text.Split(Environment.NewLine).Select(s =>
        {
            var length = s.Length;
            var lines = Math.Max((int)Math.Ceiling(length / (double)lineLength), 1);
            var paddedLen = lineLength * lines;
            return length >= paddedLen ? s : s.PadRight(paddedLen);
        }));

        return combined.Length >= text_length ? combined[..text_length] : combined.PadRight(text_length);
    }

    private static void buildKeyTables(Dictionary<char, int> charToCode, char[] codeToChar)
    {
        Array.Fill(codeToChar, '?');

        add(' ', 0);
        add('!', 1);
        add('"', 2);
        add('#', 3);
        add('$', 4);
        add('%', 5);
        add('&', 6);
        add('\'', 7);
        add('(', 8);
        add(')', 9);
        add('*', 10);
        add('+', 11);
        add(',', 12);
        add('-', 13);
        add('.', 14);
        add('/', 15);
        add('0', 16);
        add('1', 17);
        add('2', 18);
        add('3', 19);
        add('4', 20);
        add('5', 21);
        add('6', 22);
        add('7', 23);
        add('8', 24);
        add('9', 25);
        add(':', 26);
        add(';', 27);
        add('<', 28);
        add('=', 29);
        add('>', 30);
        add('?', 31);
        add('@', 32);
        add('A', 33);
        add('B', 34);
        add('C', 35);
        add('D', 36);
        add('E', 37);
        add('F', 38);
        add('G', 39);
        add('H', 40);
        add('I', 41);
        add('J', 42);
        add('K', 43);
        add('L', 44);
        add('M', 45);
        add('N', 46);
        add('O', 47);
        add('P', 48);
        add('Q', 49);
        add('R', 50);
        add('S', 51);
        add('T', 52);
        add('U', 53);
        add('V', 54);
        add('W', 55);
        add('X', 56);
        add('Y', 57);
        add('Z', 58);
        add('[', 59);
        add('\\', 60);
        add(']', 61);
        add('^', 62);
        add('_', 63);
        add('`', 64);
        add('a', 65);
        add('b', 66);
        add('c', 67);
        add('d', 68);
        add('e', 69);
        add('f', 70);
        add('g', 71);
        add('h', 72);
        add('i', 73);
        add('j', 74);
        add('k', 75);
        add('l', 76);
        add('m', 77);
        add('n', 78);
        add('o', 79);
        add('p', 80);
        add('q', 81);
        add('r', 82);
        add('s', 83);
        add('t', 84);
        add('u', 85);
        add('v', 86);
        add('w', 87);
        add('x', 88);
        add('y', 89);
        add('z', 90);
        add('{', 91);
        add('|', 92);
        add('}', 93);
        add('~', 94);
        add('€', 95);
        add('ぬ', 127);
        add('ふ', 129);
        add('あ', 130);
        add('う', 131);
        add('え', 132);
        add('お', 133);
        add('や', 134);
        add('ゆ', 135);
        add('よ', 136);
        add('わ', 137);
        add('を', 138);
        add('ほ', 139);
        add('へ', 140);
        add('た', 141);
        add('て', 142);
        add('い', 143);
        add('す', 144);
        add('か', 145);
        add('ん', 146);
        add('な', 147);
        add('に', 148);
        add('ら', 149);
        add('せ', 150);
        add('ち', 151);
        add('と', 152);
        add('し', 153);
        add('は', 154);
        add('き', 155);
        add('く', 156);
        add('ま', 157);
        add('の', 158);
        add('り', 159);
        add('れ', 160);
        add('け', 161);
        add('む', 162);
        add('つ', 163);
        add('さ', 164);
        add('そ', 165);
        add('ひ', 166);
        add('こ', 167);
        add('み', 168);
        add('も', 169);
        add('ね', 170);
        add('る', 171);
        add('め', 172);
        add('ろ', 173);
        add('。', 174);
        add('ぶ', 175);
        add('ぷ', 176);
        add('ぼ', 177);
        add('ぽ', 178);
        add('べ', 179);
        add('ぺ', 180);
        add('だ', 181);
        add('で', 182);
        add('ず', 183);
        add('が', 184);
        add('ぜ', 185);
        add('ぢ', 186);
        add('ど', 187);
        add('じ', 188);
        add('ば', 189);
        add('ぱ', 190);
        add('ぎ', 191);
        add('ぐ', 192);
        add('げ', 193);
        add('づ', 194);
        add('ざ', 195);
        add('ぞ', 196);
        add('び', 197);
        add('ぴ', 198);
        add('ご', 199);
        add('ぁ', 200);
        add('ぃ', 201);
        add('ぅ', 202);
        add('ぇ', 203);
        add('ぉ', 204);
        add('ゃ', 205);
        add('ゅ', 206);
        add('ょ', 207);
        add('ヌ', 208);
        add('フ', 209);
        add('ア', 210);
        add('ウ', 211);
        add('エ', 212);
        add('オ', 213);
        add('ヤ', 214);
        add('ユ', 215);
        add('ヨ', 216);
        add('ワ', 217);
        add('ヲ', 218);
        add('ホ', 219);
        add('ヘ', 220);
        add('タ', 221);
        add('テ', 222);
        add('イ', 223);
        add('ス', 224);
        add('カ', 225);
        add('ン', 226);
        add('ナ', 227);
        add('ニ', 228);
        add('ラ', 229);
        add('セ', 230);
        add('チ', 231);
        add('ト', 232);
        add('シ', 233);
        add('ハ', 234);
        add('キ', 235);
        add('ク', 236);
        add('マ', 237);
        add('ノ', 238);
        add('リ', 239);
        add('レ', 240);
        add('ケ', 241);
        add('ム', 242);
        add('ツ', 243);
        add('サ', 244);
        add('ソ', 245);
        add('ヒ', 246);
        add('コ', 247);
        add('ミ', 248);
        add('モ', 249);
        add('ネ', 250);
        add('ル', 251);
        add('メ', 252);
        add('ロ', 253);
        add('〝', 254);
        add('°', 255);
        return;

        void add(char ch, int code)
        {
            charToCode[ch] = code;
            if (code is >= 0 and < 256) codeToChar[code] = ch;
        }
    }

    private enum KATSetting
    {
        SyncParams,
        LineLength,
        LineCount,
        UpdateDelay
    }

    private enum KATParameter
    {
        Pointer,
        CharSync,
        Visible
    }
}