namespace Models.Services.Yaggit.Contracts;

public record GitCommit(
    string Hash,
    string Author,
    DateTime Date,
    string Message
);