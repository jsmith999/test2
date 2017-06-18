namespace System.ComponentModel.DataAnnotations {
    // Net 3.5
    
    public class StringLengthAttribute : ValidationAttribute {
        public StringLengthAttribute(int minLength, int maxLength)
            : this(maxLength) {
            MinLength = minLength;
        }

        public StringLengthAttribute(int maxLength) {
            MaxLength = maxLength;
        }

        public int MaxLength { get; private set; }
        public int MinLength { get; private set; }

        public override string Validate(object target) {
            var value = (target as string) ?? string.Empty;
            if (value.Length < MinLength) return "At least " + MinLength + " characters required";
            if (value.Length > MaxLength) return "At most " + MaxLength + " characters are accepted";
            return string.Empty;
        }
    }

    public abstract class ValidationAttribute : Attribute {
        public abstract string Validate(object target);
    }
}
