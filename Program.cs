using RESTFULL_API;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var userAddedCreditScores = new List<CreditScore>();

app.MapGet("/creditscore", () =>
{
	try
	{
		SystemIdentifier systemIdentifier = new SystemIdentifier();

		Console.WriteLine("CPU Identifier: " + systemIdentifier.CPUIdentifier);
		Console.WriteLine("Disk Serial Number: " + systemIdentifier.DiskSerialNumber);
		Console.WriteLine("BIOS Identifier: " + systemIdentifier.BIOSIdentifier);

		string input = systemIdentifier.ToJson();
		return input;
	}
	catch (PlatformNotSupportedException ex)
	{
		Console.WriteLine("Error: " + ex.Message);
		return ex.Message;
	}
});

/*app.MapPost("/creditscore", (CreditScore score) => {
	userAddedCreditScores.Add(score);
	return score;
});

app.MapGet("/userAddedCreditScores", () => userAddedCreditScores);*/

app.Run("http://localhost:3000");

record CreditScore(int Score)
{
	public string? CreditRating
	{
		get => Score switch
		{
			>= 800 => "Excellent",
			>= 700 => "Good",
			>= 600 => "Fair",
			>= 500 => "Poor",
			_ => "Bad"
		};
	}
}