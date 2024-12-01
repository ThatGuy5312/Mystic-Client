using UnityEngine.Windows.Speech;
using UnityEngine;
using MysticClient.Menu;
using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using MysticClient.Notifications;
using UnityEngine.Playables;
using MysticClient.Utils;

namespace MysticClient.Classes
{
    [Obsolete("This class will not be used not it's meant to maybe used in the future")]
    public class VoiceCommands : MonoBehaviour
    {
        private static KeywordRecognizer enablePhrase;
        private static KeywordRecognizer modPhrase;

        private static string[] phrase = { "mystic", "client", "jarvis", "google", "siri", "alaxa", "console", "bitch" };

        public static void Enable()
        {
            enablePhrase = new KeywordRecognizer(phrase);
            enablePhrase.OnPhraseRecognized += Recognition;
            enablePhrase.Start();
        }

        private static void Recognition(PhraseRecognizedEventArgs args)
        {
            enablePhrase.Stop();
            string[] btnnames = { "nevermind", "cancel", "never mind", "stop" };
            foreach (var btns in Buttons.buttons)
                foreach (var btn in btns)
                    if (btn.buttonText.Contains("["))
                    {
                        var n = btn.buttonText.Split("[");
                        btnnames.AddItem(n[0]);
                    }
                    else if (btn.buttonText.Contains(":"))
                    {
                        var n = btn.buttonText.Split(":");
                        btnnames.AddItem(n[0]);
                    }
            modPhrase = new KeywordRecognizer(btnnames);
            modPhrase.OnPhraseRecognized += RunCommand;
            modPhrase.Start();
            NotifiLib.SendNotification(NotifUtils.Voice() + "Listening...");
        }

        private static void RunCommand(PhraseRecognizedEventArgs args)
        {
            var output = args.text;
            if (output == Main.ATS(new string[] { "nevermind", "cancel", "never mind", "stop" }))
            {
                Cancel();
                return;
            }

            string target = null;
            var match = false;

            string btnname = null;

            foreach (var btns in Buttons.buttons)
                foreach (var btn in btns)
                {
                    if (match)
                        break;
                    if (btn.buttonText.Contains("["))
                    {
                        var b = btn.buttonText.Split('[');
                        btnname = b[0];
                        
                    }
                    else if (btn.buttonText.Contains(":"))
                    {
                        var s = btn.buttonText.Split(":");
                        btnname = s[0];
                    }
                    if (output.ToLower() == btnname.ToLower())
                    {
                        target = btn.buttonText;
                        match = true;
                    }
                    else
                    {
                        if (output.Contains(btnname.ToLower()))
                            target = btn.buttonText;
                    }
                }
            if (target != null)
            {
                var btn = Main.GetIndex(target);
                NotifiLib.SendNotification(NotifUtils.Voice() + (btn.enabled ? "Disabling " : "Enabling ") + btnname + "...");
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Voice() + "No Command Found");
            }
        }

        private static void Cancel()
        {
            enablePhrase.Stop();
            modPhrase.Stop();
            NotifiLib.SendNotification(NotifUtils.Voice() + "Cancelling...");
        }
    }
}