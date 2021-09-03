// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ORIGINAL: https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/Microsoft.Extensions.Configuration.UserSecrets/src/PathHelper.cs

using System;
using System.Globalization;
using System.IO;

[assembly:CLSCompliant(false)]
namespace Serious.Secrets
{
    /// <summary>
    /// Provides paths for user secrets configuration files.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// <para>
        /// Returns the path to the JSON file that stores user secrets.
        /// </para>
        /// <para>
        /// This uses the current user profile to locate the secrets file on disk in a location outside of source control.
        /// </para>
        /// </summary>
        /// <param name="userSecretsId">The user secret ID.</param>
        /// <returns>The full path to the secret file.</returns>
        public static string GetSecretsPathFromSecretsId(string userSecretsId)
        {
            if (userSecretsId is null)
            {
                throw new ArgumentNullException(nameof(userSecretsId), userSecretsId);
            }

            var badCharIndex = userSecretsId.IndexOfAny(Path.GetInvalidFileNameChars());
            if (badCharIndex != -1)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Invalid character {0} at index {1} in user secret",
                        userSecretsId[badCharIndex],
                        badCharIndex));
            }

            var home = Environment.GetEnvironmentVariable("HOME");

            bool isWindows = home is null;
            
            var root = home                                                         // On Mac/Linux it goes to ~/.abbot/secrets/
                ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // On Windows it goes to %APPDATA%\Abbot\Secrets\

            return isWindows
                ? Path.Combine(root, "Abbot", "Secrets", userSecretsId)
                : Path.Combine(root, ".abbot", "secrets", userSecretsId);
        }
    }
}
