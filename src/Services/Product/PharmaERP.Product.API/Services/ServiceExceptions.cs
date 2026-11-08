namespace PharmaERP.Product.API.Services;

public sealed class ConflictException(string message) : Exception(message) { }
