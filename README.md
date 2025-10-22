.NET Minimal Api authorized by Keycloak

Sample created by guidance from https://medium.com/@faulycoelho/net-web-api-with-keycloak-11e0286240b9

Keycloak configuration:
1. realm `Apis`
2. client `api1-client` with client authentication ON
3. client roles added: `admin`, `general`
4. realm users created: `user-admin`, `user-general`
5. users role mappings assignments:
    - user-admin: `api1-client/admin`
    - user-general: `api1-client/general`
6. client role `api1-client/admin` associated with role `api1-client/general`
