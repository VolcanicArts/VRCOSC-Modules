// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using org.mariuszgromada.math.mxparser;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Maths;

[ModuleTitle("Maths")]
[ModuleDescription("Define complex equations to drive avatar parameters")]
[ModuleType(ModuleType.Generic)]
public class MathsModule : Module
{
    private readonly Dictionary<List<string>, Equation> instances = new();
    private readonly List<PrimitiveElement> elements = new();

    protected override void OnPreLoad()
    {
        CreateTextBoxList(MathsSetting.Constants, "Constants", "Define your own constants to reuse in your equations\nChanges to this setting requires a module restart", Array.Empty<string>());
        CreateTextBoxList(MathsSetting.Functions, "Functions", "Define your own functions to reuse in your equations\nChanges to this setting requires a module restart", Array.Empty<string>());

        CreateCustom(MathsSetting.Equations, new EquationModuleSetting());
    }

    protected override Task<bool> OnModuleStart()
    {
        instances.Clear();
        elements.Clear();

        GetSettingValue<List<Equation>>(MathsSetting.Equations)!
            .Where(instance => !string.IsNullOrEmpty(instance.EquationString.Value) && instance.TriggerParameters.Count != 0 && instance.TriggerParameters.All(triggerParameter => !string.IsNullOrEmpty(triggerParameter.Value)) && !string.IsNullOrEmpty(instance.OutputParameter.Value))
            .ForEach(instance => instances.TryAdd(instance.TriggerParameters.Select(triggerParameter => triggerParameter.Value).ToList(), instance));
        elements.AddRange(GetSettingValue<List<string>>(MathsSetting.Constants)!.Select(constant => new Constant(constant)));
        elements.AddRange(GetSettingValue<List<string>>(MathsSetting.Functions)!.Select(function => new Function(function)));

        return Task.FromResult(true);
    }

    protected override async void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        var instance = instances.FirstOrDefault(pair => pair.Key.Contains(parameter.Name)).Value;
        if (instance is null) return;

        var expression = new Expression(instance.EquationString.Value, elements.ToArray());
        expression.disableImpliedMultiplicationMode();

        foreach (var missingArgument in expression.getMissingUserDefinedArguments())
        {
            var missingArgumentValue = await FindParameterValue(missingArgument);

            if (missingArgumentValue is null)
            {
                Log($"Could not retrieve missing argument value '{missingArgument}'");
                continue;
            }

            if (missingArgumentValue is bool boolValue)
                expression.addArguments(new Argument(missingArgument, boolValue));
            else if (missingArgumentValue is int intValue)
                expression.addArguments(new Argument(missingArgument, intValue));
            else if (missingArgumentValue is float floatValue)
                expression.addArguments(new Argument(missingArgument, floatValue));
        }

        var outputType = await FindParameterType(instance.OutputParameter.Value);

        if (outputType is null)
        {
            Log($"Could not find output parameter '{instance.OutputParameter.Value}'");
            return;
        }

        var output = expression.calculate();

        var finalValue = convertToOutputType(output, outputType.Value);
        SendParameter(instance.OutputParameter.Value, finalValue);
    }

    private object convertToOutputType(double value, TypeCode valueType)
    {
        try
        {
            return valueType switch
            {
                TypeCode.Boolean => Convert.ToBoolean(value),
                TypeCode.Int32 => Convert.ToInt32(value),
                TypeCode.Single => Convert.ToSingle(value),
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };
        }
        catch (Exception e)
        {
            Log($"Output error for value '{value}': '{e.Message}'");

            return valueType switch
            {
                TypeCode.Boolean => default(bool),
                TypeCode.Int32 => default(int),
                TypeCode.Single => default(float),
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };
        }
    }

    private enum MathsSetting
    {
        Constants,
        Functions,
        Equations
    }
}
