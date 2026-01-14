# ZAP Scanning Report

ZAP by [Checkmarx](https://checkmarx.com/).


## Summary of Alerts

| Risk Level | Number of Alerts |
| --- | --- |
| High | 0 |
| Medium | 0 |
| Low | 3 |
| Informational | 3 |




## Insights

| Level | Reason | Site | Description | Statistic |
| --- | --- | --- | --- | --- |
| Info | Informational | http://s265d01-app-api.azurewebsites.net | Percentage of responses with status code 3xx | 100 % |
| Info | Informational | http://s265d01-app-api.azurewebsites.net | Percentage of endpoints with method GET | 50 % |
| Info | Informational | http://s265d01-app-api.azurewebsites.net | Percentage of endpoints with method POST | 50 % |
| Info | Informational | http://s265d01-app-api.azurewebsites.net | Count of total endpoints | 4    |
| Info | Informational | http://s265d01-app-api.azurewebsites.net | Percentage of slow responses | 100 % |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Percentage of responses with status code 2xx | 49 % |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Percentage of responses with status code 4xx | 50 % |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Percentage of endpoints with content type application/json | 8 % |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Percentage of endpoints with content type application/problem+json | 8 % |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Percentage of endpoints with content type text/html | 16 % |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Percentage of endpoints with method GET | 100 % |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Count of total endpoints | 24    |
| Info | Informational | https://s265d01-app-api.azurewebsites.net | Percentage of slow responses | 100 % |




## Alerts

| Name | Risk Level | Number of Instances |
| --- | --- | --- |
| Insufficient Site Isolation Against Spectre Vulnerability | Low | 2 |
| Strict-Transport-Security Header Not Set | Low | 4 |
| Unexpected Content-Type was returned | Low | 4 |
| A Client Error response code was returned by the server | Informational | 26 |
| Re-examine Cache-control Directives | Informational | 2 |
| Storable and Cacheable Content | Informational | 8 |




## Alert Detail



### [ Insufficient Site Isolation Against Spectre Vulnerability ](https://www.zaproxy.org/docs/alerts/90004/)



##### Low (Medium)

### Description

Cross-Origin-Resource-Policy header is an opt-in header designed to counter side-channels attacks like Spectre. Resource should be specifically set as shareable amongst different origins.

* URL: https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3Fquery=ZAP
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations (query)`
  * Method: `GET`
  * Parameter: `Cross-Origin-Resource-Policy`
  * Attack: ``
  * Evidence: `same-site`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/openapi/v1.json
  * Node Name: `https://s265d01-app-api.azurewebsites.net/openapi/v1.json`
  * Method: `GET`
  * Parameter: `Cross-Origin-Resource-Policy`
  * Attack: ``
  * Evidence: `same-site`
  * Other Info: ``


Instances: 2

### Solution

Ensure that the application/web server sets the Cross-Origin-Resource-Policy header appropriately, and that it sets the Cross-Origin-Resource-Policy header to 'same-origin' for all web pages.
'same-site' is considered as less secured and should be avoided.
If resources must be shared, set the header to 'cross-origin'.
If possible, ensure that the end user uses a standards-compliant and modern web browser that supports the Cross-Origin-Resource-Policy header (https://caniuse.com/mdn-http_headers_cross-origin-resource-policy).

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cross-Origin-Embedder-Policy ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cross-Origin-Embedder-Policy)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 14

#### Source ID: 3

### [ Strict-Transport-Security Header Not Set ](https://www.zaproxy.org/docs/alerts/10035/)



##### Low (High)

### Description

HTTP Strict Transport Security (HSTS) is a web security policy mechanism whereby a web server declares that complying user agents (such as a web browser) are to interact with it using only secure HTTPS connections (i.e. HTTP layered over TLS/SSL). HSTS is an IETF standards track protocol and is specified in RFC 6797.

* URL: https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3Fquery=ZAP
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations (query)`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Search
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Search`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/openapi/v1.json
  * Node Name: `https://s265d01-app-api.azurewebsites.net/openapi/v1.json`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``


Instances: 4

### Solution

Ensure that your web server, application server, load balancer, etc. is configured to enforce Strict-Transport-Security.

### Reference


* [ https://cheatsheetseries.owasp.org/cheatsheets/HTTP_Strict_Transport_Security_Cheat_Sheet.html ](https://cheatsheetseries.owasp.org/cheatsheets/HTTP_Strict_Transport_Security_Cheat_Sheet.html)
* [ https://owasp.org/www-community/Security_Headers ](https://owasp.org/www-community/Security_Headers)
* [ https://en.wikipedia.org/wiki/HTTP_Strict_Transport_Security ](https://en.wikipedia.org/wiki/HTTP_Strict_Transport_Security)
* [ https://caniuse.com/stricttransportsecurity ](https://caniuse.com/stricttransportsecurity)
* [ https://datatracker.ietf.org/doc/html/rfc6797 ](https://datatracker.ietf.org/doc/html/rfc6797)


#### CWE Id: [ 319 ](https://cwe.mitre.org/data/definitions/319.html)


#### WASC Id: 15

#### Source ID: 3

### [ Unexpected Content-Type was returned ](https://www.zaproxy.org/docs/alerts/100001/)



##### Low (High)

### Description

A Content-Type of text/html was returned by the server.
This is not one of the types expected to be returned by an API.
Raised by the 'Alert on Unexpected Content Types' script

* URL: https://s265d01-app-api.azurewebsites.net/computeMetadata/v1/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/computeMetadata/v1/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `text/html`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/latest/meta-data/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/latest/meta-data/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `text/html`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/metadata/instance
  * Node Name: `https://s265d01-app-api.azurewebsites.net/metadata/instance`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `text/html`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/opc/v1/instance/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/opc/v1/instance/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `text/html`
  * Other Info: ``


Instances: 4

### Solution



### Reference




#### Source ID: 4

### [ A Client Error response code was returned by the server ](https://www.zaproxy.org/docs/alerts/100000/)



##### Informational (High)

### Description

A response code of 404 was returned by the server.
This may indicate that the application is failing to handle unexpected input correctly.
Raised by the 'Alert on HTTP Response Code Error' script

* URL: http://s265d01-app-api.azurewebsites.net/api/Courses/instanceId
  * Node Name: `http://s265d01-app-api.azurewebsites.net/api/Courses/instanceId`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net
  * Node Name: `https://s265d01-app-api.azurewebsites.net`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/8373501138533912162
  * Node Name: `https://s265d01-app-api.azurewebsites.net/8373501138533912162`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/actuator/health
  * Node Name: `https://s265d01-app-api.azurewebsites.net/actuator/health`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/8723073428298282105
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/8723073428298282105`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3F=
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `400`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3Fquery=%2527
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations (query)`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `400`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `400`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Courses
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Courses`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Courses/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Courses/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Courses/8788854286811893351
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Courses/8788854286811893351`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Search
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Search`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `405`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/api/Search/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Search/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `405`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/computeMetadata/v1/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/computeMetadata/v1/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/latest/meta-data/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/latest/meta-data/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/metadata/instance
  * Node Name: `https://s265d01-app-api.azurewebsites.net/metadata/instance`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/opc/v1/instance/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/opc/v1/instance/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/openapi
  * Node Name: `https://s265d01-app-api.azurewebsites.net/openapi`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/openapi/
  * Node Name: `https://s265d01-app-api.azurewebsites.net/openapi/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/openapi/8025655722143046034
  * Node Name: `https://s265d01-app-api.azurewebsites.net/openapi/8025655722143046034`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `404`
  * Other Info: ``
* URL: http://s265d01-app-api.azurewebsites.net/api/Search
  * Node Name: `http://s265d01-app-api.azurewebsites.net/api/Search ()({"query":ZAP,"sessionId":"John Doe","page":10,"pageSize":10,"location":"John Doe","radius":1.2,"orderBy":"Relevance","courseType":"John Doe","qualificationLevel":"John Doe","learningMethod":"John Doe","courseHours":"John Doe","studyTime":"John Doe"})`
  * Method: `POST`
  * Parameter: ``
  * Attack: ``
  * Evidence: `405`
  * Other Info: ``


Instances: 26

### Solution



### Reference



#### CWE Id: [ 388 ](https://cwe.mitre.org/data/definitions/388.html)


#### WASC Id: 20

#### Source ID: 4

### [ Re-examine Cache-control Directives ](https://www.zaproxy.org/docs/alerts/10015/)



##### Informational (Low)

### Description

The cache-control header has not been set properly or is missing, allowing the browser and proxies to cache content. For static assets like css, js, or image files this might be intended, however, the resources should be reviewed to ensure that no sensitive content will be cached.

* URL: https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3Fquery=ZAP
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations (query)`
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-api.azurewebsites.net/openapi/v1.json
  * Node Name: `https://s265d01-app-api.azurewebsites.net/openapi/v1.json`
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``


Instances: 2

### Solution

For secure content, ensure the cache-control HTTP header is set with "no-cache, no-store, must-revalidate". If an asset should be cached consider setting the directives "public, max-age, immutable".

### Reference


* [ https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#web-content-caching ](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#web-content-caching)
* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cache-Control ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cache-Control)
* [ https://grayduck.mn/2021/09/13/cache-control-recommendations/ ](https://grayduck.mn/2021/09/13/cache-control-recommendations/)


#### CWE Id: [ 525 ](https://cwe.mitre.org/data/definitions/525.html)


#### WASC Id: 13

#### Source ID: 3

### [ Storable and Cacheable Content ](https://www.zaproxy.org/docs/alerts/10049/)



##### Informational (Medium)

### Description

The response contents are storable by caching components such as proxy servers, and may be retrieved directly from the cache, rather than from the origin server by the caching servers, in response to similar requests from other users. If the response data is sensitive, personal or user-specific, this may result in sensitive information being leaked. In some cases, this may even result in a user gaining complete control of the session of another user, depending on the configuration of the caching components in use in their environment. This is primarily an issue where "shared" caching servers such as "proxy" caches are configured on the local network. This configuration is typically found in corporate or educational environments, for instance.

* URL: http://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3Fquery=ZAP
  * Node Name: `http://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations (query)`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: http://s265d01-app-api.azurewebsites.net/api/Courses/instanceId
  * Node Name: `http://s265d01-app-api.azurewebsites.net/api/Courses/instanceId`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3Fquery=ZAP
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations (query)`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Courses/instanceId`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://s265d01-app-api.azurewebsites.net/api/Search
  * Node Name: `https://s265d01-app-api.azurewebsites.net/api/Search`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://s265d01-app-api.azurewebsites.net/openapi/v1.json
  * Node Name: `https://s265d01-app-api.azurewebsites.net/openapi/v1.json`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: http://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations%3Fquery=ZAP
  * Node Name: `http://s265d01-app-api.azurewebsites.net/api/AutocompleteLocations (query)`
  * Method: `POST`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: http://s265d01-app-api.azurewebsites.net/api/Search
  * Node Name: `http://s265d01-app-api.azurewebsites.net/api/Search ()({"query":ZAP,"sessionId":"John Doe","page":10,"pageSize":10,"location":"John Doe","radius":1.2,"orderBy":"Relevance","courseType":"John Doe","qualificationLevel":"John Doe","learningMethod":"John Doe","courseHours":"John Doe","studyTime":"John Doe"})`
  * Method: `POST`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`


Instances: 8

### Solution

Validate that the response does not contain sensitive, personal or user-specific information. If it does, consider the use of the following HTTP response headers, to limit, or prevent the content being stored and retrieved from the cache by another user:
Cache-Control: no-cache, no-store, must-revalidate, private
Pragma: no-cache
Expires: 0
This configuration directs both HTTP 1.0 and HTTP 1.1 compliant caching servers to not store the response, and to not retrieve the response (without validation) from the cache, in response to a similar request.

### Reference


* [ https://datatracker.ietf.org/doc/html/rfc7234 ](https://datatracker.ietf.org/doc/html/rfc7234)
* [ https://datatracker.ietf.org/doc/html/rfc7231 ](https://datatracker.ietf.org/doc/html/rfc7231)
* [ https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html ](https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html)


#### CWE Id: [ 524 ](https://cwe.mitre.org/data/definitions/524.html)


#### WASC Id: 13

#### Source ID: 3


