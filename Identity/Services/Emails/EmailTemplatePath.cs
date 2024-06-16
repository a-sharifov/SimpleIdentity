
namespace Identity.Services.Emails;

/// <summary>
/// This class is used to get the path of the email templates.
/// </summary>
internal static class EmailTemplatePath
{
    /// <summary>
    /// required change this template: {{userName}}, {{confirmationLink}}
    /// </summary>
    public static string ConfirmEmailTemplate => GetTemplatePath("ConfirmEmailTemplate.html");

    private static string GetTemplatePath(string templateName) =>
        Path.Combine(Templates.AssemblyReference.AssemblyPath, templateName);
}
