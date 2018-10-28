using Plugin.TextToSpeech.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plugin.TextToSpeech
{
    /// <summary>
    /// Text To Speech Impelemenatation Windows
    /// </summary>
    public class TextToSpeech : ITextToSpeech, IDisposable
    {
        /// <summary>
        /// Speak back text
        /// </summary>
        /// <param name="text">Text to speak</param>
        /// <param name="crossLocale">Locale of voice</param>
        /// <param name="pitch">Pitch of voice</param>
        /// <param name="speakRate">Speak Rate of voice (All) (0.0 - 2.0f)</param>
        /// <param name="volume">Volume of voice (0.0-1.0)</param>
        /// <param name="cancelToken">Canelation token to stop speak</param>
        /// <exception cref="ArgumentNullException">Thrown if text is null</exception>
        /// <exception cref="ArgumentException">Thrown if text length is greater than maximum allowed</exception>
        public Task Speak(string text, CrossLocale? crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken cancelToken = default(CancellationToken))
        {
                throw new ArgumentNullException("Text can not be null");
        }

        /// <summary>
        /// Get all installed and valid languages
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CrossLocale>> GetInstalledLanguages() => null;


        /// <summary>
        /// Gets the max string length of the speech engine
        /// -1 means no limit
        /// </summary>
        public int MaxSpeechInputLength => -1;

        /// <summary>
        /// Dispose of TTS
        /// </summary>
        public void Dispose()
        {
        }
          
    }
}
