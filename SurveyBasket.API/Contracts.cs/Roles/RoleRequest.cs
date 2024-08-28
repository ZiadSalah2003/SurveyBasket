namespace SurveyBasket.API.Contracts.cs.Roles
{
	public record RoleRequest(
		string Name,
		IList<string> Permissions
	);
}
