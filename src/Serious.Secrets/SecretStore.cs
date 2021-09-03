using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serious.IO;

namespace Serious.Secrets
{
    public class SecretStore : ISecretStore
    {
        readonly IFileSystem _fileSystem;
        readonly IFileInfo _secretFile;
        readonly ISecretProtector _protector;
        bool _loaded = true;
        readonly string _secretsFilePath;
        readonly IDictionary<string, string> _secrets = new Dictionary<string, string>();
        
        public SecretStore(
            string secretsFilePath,
            IFileSystem fileSystem,
            ISecretProtector protector)
        {
            _fileSystem = fileSystem;
            _protector = protector;
            _secretsFilePath = secretsFilePath;
            _secretFile = _fileSystem.GetFile(_secretsFilePath);
        }

        public async Task LoadAsync()
        {
            _secrets.Clear();
            var secretDir = Path.GetDirectoryName(_secretsFilePath)!;
            _fileSystem.CreateDirectory(secretDir);
            if (_secretFile.Exists)
            {
                using var reader = _secretFile.OpenText();
                while (await reader.ReadLineAsync() is { Length: > 0 } line)
                {
                    var secret = line.Split('|');
                    if (secret.Length != 2)
                    {
                        throw new InvalidOperationException($"The secret file {_secretsFilePath} has been corrupted.");
                    }

                    var key = Decode(secret[0]);
                    var value = _protector.Unprotect(secret[1]);
                    if (value is not null)
                    {
                        _secrets.Add(key, value);
                    }
                }
            }

            _loaded = true;
        }

        public async Task SaveAsync()
        {
            await using var writer = _secretFile.OpenWriter();
            // We want to keep our dependencies minimal and for storing secrets, it seems a PIPE delimited file 
            // is plenty fine. No need to use JSON for this.
            foreach (var (key, value) in _secrets)
            {
                await writer.WriteLineAsync($"{Encode(key)}|{_protector.Protect(value)}");
            }
        }

        public void SetSecret(string key, string value)
        {
            _secrets[key] = value;
        }
        
        /// <summary>
        /// Retrieves a secret with the specified key.
        /// </summary>
        /// <param name="key">The key for the secret.</param>
        public string? this[string key]
        {
            get
            {
                if (!_loaded)
                {
                    throw new InvalidOperationException("Make sure to load the Secret store before accessing it.");
                }

                return _secrets.TryGetValue(key, out var value)
                    ? value
                    : null;
            }
        }

        static string Decode(string encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }

        static string Encode(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }
    }
}