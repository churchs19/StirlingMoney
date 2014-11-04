using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalSettingsService : ISettingsService
    {
        private const string passwordDescriptor = "LOCAL=StirlingMoneyPassword";

        public async Task<string> EncryptPassword(string value)
        {
            // Create a DataProtectionProvider object for the specified descriptor.
            DataProtectionProvider Provider = new DataProtectionProvider(passwordDescriptor);

            // Encode the plaintext input message to a buffer.
            var encoding = BinaryStringEncoding.Utf8;
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(value, encoding);

            // Encrypt the message.
            IBuffer buffProtected = await Provider.ProtectAsync(buffMsg);

            return CryptographicBuffer.ConvertBinaryToString(encoding, buffProtected);
        }

        public async Task<string> DecryptPassword(string value)
        {
            // Create a DataProtectionProvider object.
            DataProtectionProvider Provider = new DataProtectionProvider(passwordDescriptor);

            var encoding = BinaryStringEncoding.Utf8;
            IBuffer buffProtected = CryptographicBuffer.ConvertStringToBinary(value, encoding);

            // Decrypt the protected message specified on input.
            IBuffer buffUnprotected = await Provider.UnprotectAsync(buffProtected);

            // Execution of the SampleUnprotectData method resumes here
            // after the awaited task (Provider.UnprotectAsync) completes
            // Convert the unprotected message from an IBuffer object to a string.
            return CryptographicBuffer.ConvertBinaryToString(encoding, buffUnprotected);
        }

        public bool SaveSetting<T>(T value, string key)
        {
            bool valueChanged = false;

            var data = value;
            if (key.ToLower() == "password" && typeof(T) == typeof(string))
            {
                data = (T)Convert.ChangeType(EncryptPassword(value.ToString()), typeof(T));
                ApplicationData.Current.LocalSettings.Values[key] = data;
                valueChanged = true;
            }
            else
            {
                var settings = ApplicationData.Current.RoamingSettings.Values;

                // If the key exists
                if (settings.ContainsKey(key))
                {
                    // If the value has changed
                    if (settings[key] is T)
                    {
                        T currentVal = (T)settings[key];
                        if (!currentVal.Equals(data))
                        {
                            // Store the new value
                            settings[key] = data;
                            valueChanged = true;
                        }
                    }
                    else
                    {
                        settings[key] = data;
                        valueChanged = true;
                    }
                }
                // Otherwise create the key.
                else
                {
                    settings.Add(key, data);
                    valueChanged = true;
                }                
            }
            return valueChanged;
        }

        public T LoadSetting<T>(string key)
        {
            try
            {
                if (key.ToLower() == "password" && typeof(T) == typeof(string))
                {
                    var settings = ApplicationData.Current.LocalSettings.Values;
                    if (settings.ContainsKey(key))
                    {
                        var value = (T)settings[key];
                        return (T)Convert.ChangeType(DecryptPassword(value.ToString()), typeof(T));
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    var settings = ApplicationData.Current.RoamingSettings.Values;
                    
                    // If the key exists, retrieve the value.
                    if (settings.ContainsKey(key))
                    {
                            return (T)settings[key];
                    }
                    // Otherwise, use the default value.
                    else
                    {
                        return default(T);
                    }
                }
            }
            catch
            {
                return default(T);
            }
        }

        public bool RemoveSetting(string key)
        {
            var removed = false;
            if(key.ToLower() == "password")
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                {
                    ApplicationData.Current.LocalSettings.Values.Remove(key);
                    removed = true;
                }
            } 
            else if(ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.RoamingSettings.Values.Remove(key);
                removed = true;
            }

            return removed;
        }
    }
}
