﻿/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

namespace IdentityModel.Client
{
    public static partial class OAuth2ClientExtensions
    {
        public static Task<TokenResponse> RequestClientCredentialsAsync(this OAuth2Client client, string scope = null, Dictionary<string, string> additionalValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.ClientCredentials }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, additionalValues), cancellationToken);
        }

        public static Task<TokenResponse> RequestResourceOwnerPasswordAsync(this OAuth2Client client, string userName, string password, string scope = null, Dictionary<string, string> additionalValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.Password },
                { OAuth2Constants.UserName, userName },
                { OAuth2Constants.Password, password }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, additionalValues), cancellationToken);
        }

        public static Task<TokenResponse> RequestAuthorizationCodeAsync(this OAuth2Client client, string code, string redirectUri, Dictionary<string, string> additionalValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.AuthorizationCode },
                { OAuth2Constants.Code, code },
                { OAuth2Constants.RedirectUri, redirectUri }
            };

            return client.RequestAsync(Merge(client, fields, additionalValues), cancellationToken);
        }

        public static Task<TokenResponse> RequestRefreshTokenAsync(this OAuth2Client client, string refreshToken, Dictionary<string, string> additionalValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.RefreshToken },
                { OAuth2Constants.RefreshToken, refreshToken }
            };

            return client.RequestAsync(Merge(client, fields, additionalValues), cancellationToken);
        }

        public static Task<TokenResponse> RequestAssertionAsync(this OAuth2Client client, string assertionType, string assertion, string scope = null, Dictionary<string, string> additionalValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, assertionType },
                { OAuth2Constants.Assertion, assertion },
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, additionalValues), cancellationToken);
        }

        public static Task<TokenResponse> RequestCustomGrantAsync(this OAuth2Client client, string grantType, string scope = null, Dictionary<string, string> additionalValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, grantType }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, additionalValues), cancellationToken);
        }

        public static Task<TokenResponse> RequestCustomAsync(this OAuth2Client client, Dictionary<string, string> values, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RequestAsync(Merge(client, values), cancellationToken);
        }

        private static Dictionary<string, string> Merge(OAuth2Client client, Dictionary<string, string> explicitValues, Dictionary<string, string> additionalValues = null)
        {
            var merged = explicitValues;

            if (client.AuthenticationStyle == OAuth2Client.ClientAuthenticationStyle.PostValues)
            {
                merged.Add(OAuth2Constants.ClientId, client.ClientId);

                if (!string.IsNullOrEmpty(client.ClientSecret))
                {
                    merged.Add(OAuth2Constants.ClientSecret, client.ClientSecret);
                }
            }

            if (additionalValues != null)
            {
                merged =
                    explicitValues.Concat(additionalValues.Where(add => !explicitValues.ContainsKey(add.Key)))
                                         .ToDictionary(final => final.Key, final => final.Value);
            }

            return merged;
        }

        private static Dictionary<string, string> ObjectToDictionary(object values)
        {
            var dictionary = values as Dictionary<string, string>;
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<string, string>();

            //foreach (var prop in values.GetType().GetProperties())
            //{
            //    var value = prop.GetValue(values) as string;
            //    if (!string.IsNullOrEmpty(value))
            //    {
            //        dictionary.Add(prop.Name, value);
            //    }
            //}

            //var dic = from property in values.GetType().GetProperties()
            //  select new KeyValuePair<string,object>(property.Name, property.GetValue(oParams,null));
    

            return dictionary;
        }
    }
}