namespace EnterpriseCommerceCore.App.Step05_Composition
{
    // VALUE OBJECT: no identity of its own; defined entirely by its values.
    // 'record' gives us value-based equality and immutability-friendly features for free.
    // An Address belongs to ONE order's shipping details: it is a part of the order.
    // Because a record is immutable, even if the same Address value appears elsewhere,
    // nobody can change it from under the order. (Preview of DDD "Value Objects".)
    public sealed record Address(string Street, string City, string PostalCode, string Country);
}