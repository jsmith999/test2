namespace System.ComponentModel.DataAnnotations {
    // NET 3.5
    public class ColumnAttribute : Attribute {
        public ColumnAttribute(string name) {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
