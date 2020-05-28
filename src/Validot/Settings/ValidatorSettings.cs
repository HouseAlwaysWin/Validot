namespace Validot.Settings
{
    using System.Collections.Generic;

    using Validot.Factory;
    using Validot.Settings.Capacities;
    using Validot.Translations;

    public class ValidatorSettings
    {
        private static readonly DisabledCapacityInfo _disabledCapacityInfo = new DisabledCapacityInfo();

        private readonly TranslationCompiler _translationCompiler = new TranslationCompiler();

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations
        {
            get => _translationCompiler.Translations;
        }

        public bool? ReferenceLoopProtection { get; private set; }

        internal ICapacityInfo CapacityInfo { get; private set; } = _disabledCapacityInfo;

        public static ValidatorSettings GetDefault()
        {
            var settings = new ValidatorSettings().WithEnglishTranslation();

            return settings;
        }

        public ValidatorSettings WithReferenceLoopProtection()
        {
            ReferenceLoopProtection = true;

            return this;
        }

        public ValidatorSettings WithReferenceLoopProtectionDisabled()
        {
            ReferenceLoopProtection = false;

            return this;
        }

        public ValidatorSettings WithTranslation(string name, string messageKey, string translation)
        {
            _translationCompiler.Add(name, messageKey, translation);

            return this;
        }

        public ValidatorSettings WithTranslation(string name, IReadOnlyDictionary<string, string> translation)
        {
            ThrowHelper.NullArgument(name, nameof(name));
            ThrowHelper.NullArgument(translation, nameof(translation));

            foreach (var entry in translation)
            {
                _translationCompiler.Add(name, entry.Key, entry.Value);
            }

            return this;
        }

        public ValidatorSettings WithTranslation(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
        {
            ThrowHelper.NullArgument(translations, nameof(translations));

            foreach (var translation in translations)
            {
                _translationCompiler.Add(translation.Key, translation.Value);
            }

            return this;
        }

        public ValidatorSettings WithTranslation(ITranslationsHolder translationsHolder)
        {
            ThrowHelper.NullArgument(translationsHolder, nameof(translationsHolder));

            _translationCompiler.Add(translationsHolder.Translations);

            return this;
        }

        public ValidatorSettings WithCapacityInfo(ICapacityInfo capacityInfo)
        {
            ThrowHelper.NullArgument(capacityInfo, nameof(capacityInfo));

            CapacityInfo = capacityInfo;

            return this;
        }
    }
}
