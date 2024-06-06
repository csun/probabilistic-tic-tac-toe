namespace PTTT
{
    public enum SquareContents
    {
        Empty,
        X,
        O
    }

    public static class SquareContentsHelper
    {
        public static SquareContents FromString(string s)
        {
            var lowerText = s.ToLower();
            switch (lowerText)
            {
                case "x":
                    return SquareContents.X;
                case "o":
                    return SquareContents.O;
                default:
                    return SquareContents.Empty;
            }
        }
    }
}
