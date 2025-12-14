// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

namespace VRCOSC.Modules.Twitch;

using System.Net;
using System.Text;

public static class TwitchAccessTokenListener
{
  private static CancellationTokenSource? cancellationTokenSource;

  public static void Start()
  {
    const string prefix = "http://localhost:5555/";
    var listener = new HttpListener();
    listener.Prefixes.Add(prefix);
    listener.Start();

    cancellationTokenSource = new CancellationTokenSource();

    Task.Run(async () =>
    {
      while (!cancellationTokenSource.IsCancellationRequested)
      {
        try
        {
          var ctx = await listener.GetContextAsync().WaitAsync(cancellationTokenSource.Token);
          handle(ctx);
        }
        catch
        {
          continue;
        }
      }

      listener.Stop();
    });
  }

  public static void Stop()
  {
    cancellationTokenSource?.Cancel();
  }

  private static void handle(HttpListenerContext ctx)
  {
    try
    {
      var path = ctx.Request.Url?.AbsolutePath;

      if (path == "/favicon.ico")
      {
        ctx.Response.StatusCode = 204;
        ctx.Response.Close();
        return;
      }

      var bytes = Encoding.UTF8.GetBytes(html_page);

      ctx.Response.StatusCode = 200;
      ctx.Response.ContentType = "text/html; charset=utf-8";
      ctx.Response.ContentLength64 = bytes.Length;

      ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
      ctx.Response.OutputStream.Close();
    }
    catch
    {
      // Avoid crashing the listener on client disconnects
      try
      {
        ctx.Response.Abort();
      }
      catch
      {
      }
    }
  }

  private const string html_page = """
                                   <!doctype html>
                                   <html lang="en">
                                   <head>
                                     <meta charset="utf-8" />
                                     <meta name="viewport" content="width=device-width, initial-scale=1" />
                                     <title>Fragment Viewer</title>
                                     <style>
                                       html, body {
                                         height: 100%;
                                         margin: 0;
                                         font-family: system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif;
                                         background: #0b0f17;
                                         color: #e7eefc;
                                       }
                                       .wrap {
                                         min-height: 100%;
                                         display: grid;
                                         place-items: center;
                                         padding: 24px;
                                       }
                                       .card {
                                         width: min(720px, 100%);
                                         background: rgba(255,255,255,0.06);
                                         border: 1px solid rgba(255,255,255,0.12);
                                         border-radius: 16px;
                                         padding: 22px;
                                         box-shadow: 0 12px 40px rgba(0,0,0,0.35);
                                         text-align: center;
                                       }
                                       h1 {
                                         margin: 0 0 12px;
                                         font-size: 18px;
                                         font-weight: 600;
                                         opacity: 0.9;
                                       }
                                       .value {
                                         margin: 0;
                                         padding: 16px;
                                         border-radius: 12px;
                                         background: rgba(0,0,0,0.25);
                                         border: 1px solid rgba(255,255,255,0.10);
                                         font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
                                         font-size: 16px;
                                         word-break: break-word;
                                         user-select: all;
                                       }
                                       .actions {
                                         margin-top: 14px;
                                         display: flex;
                                         gap: 10px;
                                         justify-content: center;
                                         flex-wrap: wrap;
                                       }
                                       button {
                                         border: 0;
                                         border-radius: 10px;
                                         padding: 10px 14px;
                                         font-weight: 600;
                                         cursor: pointer;
                                         background: #2b6cff;
                                         color: white;
                                       }
                                       button.secondary {
                                         background: rgba(255,255,255,0.10);
                                         color: #e7eefc;
                                         border: 1px solid rgba(255,255,255,0.14);
                                       }
                                       .hint {
                                         margin-top: 12px;
                                         font-size: 13px;
                                         opacity: 0.75;
                                       }
                                       .toast {
                                         margin-top: 10px;
                                         font-size: 13px;
                                         min-height: 1.2em;
                                       }
                                     </style>
                                   </head>
                                   <body>
                                     <div class="wrap">
                                       <div class="card">
                                         <h1>Twitch Access Token</h1>
                                         <p id="frag" class="value">(none)</p>

                                         <div class="actions">
                                           <button id="copyBtn">Copy to clipboard</button>
                                         </div>
                                   
                                         <div class="toast" id="toast"></div>
                                         <div class="hint">
                                           Copy the code into the 'Access Token' textbox
                                         </div>
                                       </div>
                                     </div>

                                     <script>
                                     const valueEl = document.getElementById("frag");
                                     const toastEl = document.getElementById("toast");
                                   
                                     function parseFragmentParams() {
                                       const h = (location.hash || "").replace(/^#/, "");
                                       const params = new URLSearchParams(h);
                                       return params;
                                     }
                                   
                                     function getWantedValue() {
                                       const params = parseFragmentParams();
                                   
                                       const accessToken = params.get("access_token");
                                       if (accessToken) return accessToken;
                                     }
                                   
                                     function render() {
                                       const v = getWantedValue();
                                       valueEl.textContent = v ? v : "(none)";
                                       toastEl.textContent = "";
                                     }
                                   
                                     async function copyText(text) {
                                         await navigator.clipboard.writeText(text);
                                         return true;
                                     }
                                   
                                     document.getElementById("copyBtn").addEventListener("click", async () => {
                                       const ok = await copyText(getWantedValue());
                                       toastEl.textContent = ok ? "Copied!" : "Copy failed.";
                                     });
                                   
                                     addEventListener("hashchange", render);
                                     render();
                                   </script>
                                   
                                   </body>
                                   </html>
                                   """;
}