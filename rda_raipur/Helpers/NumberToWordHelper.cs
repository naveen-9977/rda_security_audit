using System;

namespace rda_raipur.Helpers
{
    public static class NumberToWordHelper
    {
        public static string ConvertToWords(long number, bool isEnglish = true)
        {
            if (number == 0) return isEnglish ? "Zero Only" : "शून्य रुपये केवल";

            if (isEnglish)
                return ConvertToEnglish(number) + " Only";
            else
                return ConvertToHindi(number) + " रुपये केवल";
        }

        private static string ConvertToEnglish(long number)
        {
            string words = "";
            if ((number / 10000000) > 0) { words += ConvertToEnglish(number / 10000000) + " Crore "; number %= 10000000; }
            if ((number / 100000) > 0) { words += ConvertToEnglish(number / 100000) + " Lakh "; number %= 100000; }
            if ((number / 1000) > 0) { words += ConvertToEnglish(number / 1000) + " Thousand "; number %= 1000; }
            if ((number / 100) > 0) { words += ConvertToEnglish(number / 100) + " Hundred "; number %= 100; }
            if (number > 0)
            {
                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
                if (number < 20) words += unitsMap[number];
                else { words += tensMap[number / 10]; if ((number % 10) > 0) words += " " + unitsMap[number % 10]; }
            }
            return words.Trim();
        }

        private static string ConvertToHindi(long number)
        {
            string words = "";
            string[] h = { "", "एक", "दो", "तीन", "चार", "पांच", "छह", "सात", "आठ", "नौ", "दस", "ग्यारह", "बारह", "तेरह", "चौदह", "पंद्रह", "सोलह", "सत्रह", "अठारह", "उन्नीस", "बीस", "इक्कीस", "बाईस", "तेईस", "चौबीस", "पच्चीस", "छब्बीस", "सत्ताईस", "अट्ठाईस", "उनतीस", "तीस", "इकतीस", "बत्तीस", "तैंतीस", "चौंतीस", "पैंतीस", "छत्तीस", "सैंतीस", "अड़तीस", "उनतालीस", "चालीस", "इकतालीस", "बयालीस", "तैंतालीस", "चवालीस", "पैंतालीस", "छियालीस", "सैंतालीस", "अड़तालीस", "उनचास", "पचास", "इक्यावन", "बावन", "तिरेपन", "चौवन", "पचपन", "छप्पन", "सत्तावन", "अट्ठावन", "उनसठ", "साठ", "इकसठ", "बासठ", "तिरसठ", "चौंसठ", "पैंसठ", "छियासठ", "सड़सठ", "अड़सठ", "उनहत्तर", "सत्तर", "इकहत्तर", "बहत्तर", "तिहत्तर", "चौहत्तर", "पचहत्तर", "छिहत्तर", "सतहत्तर", "अठहत्तर", "उन्नासी", "अस्सी", "इक्यासी", "बयासी", "तिरासी", "चौरासी", "पचासी", "छियासी", "सत्तासी", "अट्ठासी", "नवासी", "नब्बे", "इक्यानवे", "बानवे", "तिरानवे", "चौरानवे", "पचानवे", "छियानवे", "सत्तानवे", "अट्ठानवे", "निन्यानवे" };

            if ((number / 10000000) > 0) { words += ConvertToHindi(number / 10000000) + " करोड़ "; number %= 10000000; }
            if ((number / 100000) > 0) { words += ConvertToHindi(number / 100000) + " लाख "; number %= 100000; }
            if ((number / 1000) > 0) { words += ConvertToHindi(number / 1000) + " हजार "; number %= 1000; }
            if ((number / 100) > 0) { words += ConvertToHindi(number / 100) + " सौ "; number %= 100; }
            if (number > 0) { words += h[number] + " "; }
            return words.Trim();
        }
    }
}