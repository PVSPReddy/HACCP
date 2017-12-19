using Android.Text;
using Java.Lang;
using Java.Util.Regex;

namespace HACCP.Droid
{
    public class CharacterInputFilter : Object, IInputFilter
    {
        private readonly Pattern pattern;

        public CharacterInputFilter(string _pattern)
        {
            pattern = Pattern.Compile(_pattern);
        }

        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart,
            int dend)
        {
            var textToCheck = dest.SubSequence(0, dstart) + source.SubSequence(start, end) +
                              dest.SubSequence(dend, dest.Length());

            var matcher = pattern.Matcher(textToCheck);

            // Entered text does not match the pattern  
            if (!matcher.Matches())
            {
                // It does not match partially too  
                if (!matcher.HitEnd())
                {
                    return null;
                }
            }

            return null;
        }
    }
}