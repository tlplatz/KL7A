using System.Collections.Generic;
using System.Linq;
using KL7A.Configuration;

namespace KL7A.Utility
{
    public static class IndicatorUtil
    {
        private static void FixIndicators(IEnumerable<IHasIndicators> values)
        {
            var duplicates = values.GroupBy(g => g.Indicator).Select(c => new { Key = c.Key, Count = c.Count() }).Where(c => c.Count > 1).ToList();

            foreach (var item in duplicates)
            {
                List<IHasIndicators> temp = values.Where(v => v.Indicator == item.Key).ToList();

                temp.RemoveAt(0);

                foreach (var tempItem in temp)
                {
                    string indic = RandomUtil.GetRandomAlpha(5);
                    while (values.Any(v => v.Indicator == indic))
                    {
                        indic = RandomUtil.GetRandomAlpha(5);
                    }
                    tempItem.Indicator = indic;
                }
            }
        }
        private static void FixNumericIndicators(IEnumerable<IHasIndicators> values)
        {
            var duplicates = values.GroupBy(g => g.NumericIndicator).Select(c => new { Key = c.Key, Count = c.Count() }).Where(c => c.Count > 1).ToList();

            foreach (var item in duplicates)
            {
                List<IHasIndicators> temp = values.Where(v => v.NumericIndicator == item.Key).ToList();

                temp.RemoveAt(0);

                foreach (var tempItem in temp)
                {
                    string indic = RandomUtil.GetRandomNumeric(5);
                    while (values.Any(v => v.NumericIndicator == indic))
                    {
                        indic = RandomUtil.GetRandomNumeric(5);
                    }
                    tempItem.NumericIndicator = indic;
                }
            }
        }

        public static void FixDuplicateIndicators(IEnumerable<IHasIndicators> values)
        {
            FixIndicators(values);
            FixNumericIndicators(values);
        }

    }
}
