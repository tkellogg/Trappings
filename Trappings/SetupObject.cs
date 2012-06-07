namespace Trappings
{
    public class SetupObject
    {
        public string CollectionName { get; set; }
        public object Value { get; set; }

        public SetupObject()
        {
        }

        public SetupObject(string collectionName, object value)
        {
            CollectionName = collectionName;
            Value = value;
        }
    }
}