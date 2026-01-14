# ZAP Scanning Report

ZAP by [Checkmarx](https://checkmarx.com/).


## Summary of Alerts

| Risk Level | Number of Alerts |
| --- | --- |
| High | 0 |
| Medium | 1 |
| Low | 7 |
| Informational | 8 |




## Insights

| Level | Reason | Site | Description | Statistic |
| --- | --- | --- | --- | --- |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of responses with status code 2xx | 86 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of responses with status code 3xx | 4 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of responses with status code 4xx | 8 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with content type application/json | 4 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with content type image/png | 9 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with content type image/svg+xml | 9 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with content type image/x-icon | 4 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with content type text/css | 14 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with content type text/html | 33 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with content type text/javascript | 19 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with method GET | 90 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of endpoints with method POST | 9 % |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Count of total endpoints | 21    |
| Info | Informational | https://s265d01-app-web.azurewebsites.net | Percentage of slow responses | 91 % |




## Alerts

| Name | Risk Level | Number of Instances |
| --- | --- | --- |
| CSP: Failure to Define Directive with No Fallback | Medium | Systemic |
| Cookie No HttpOnly Flag | Low | 2 |
| Cookie Without Secure Flag | Low | 4 |
| Cookie without SameSite Attribute | Low | 2 |
| Insufficient Site Isolation Against Spectre Vulnerability | Low | Systemic |
| Permissions Policy Header Not Set | Low | Systemic |
| Private IP Disclosure | Low | 1 |
| Strict-Transport-Security Header Not Set | Low | Systemic |
| Cookie Poisoning | Informational | 2 |
| Information Disclosure - Suspicious Comments | Informational | 2 |
| Modern Web Application | Informational | Systemic |
| Non-Storable Content | Informational | 2 |
| Re-examine Cache-control Directives | Informational | Systemic |
| Session Management Response Identified | Informational | 4 |
| Storable and Cacheable Content | Informational | Systemic |
| User Controllable HTML Element Attribute (Potential XSS) | Informational | 1 |




## Alert Detail



### [ CSP: Failure to Define Directive with No Fallback ](https://www.zaproxy.org/docs/alerts/10055/)



##### Medium (High)

### Description

The Content Security Policy fails to define one of the directives that has no fallback. Missing/excluding them is the same as allowing anything.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: `Content-Security-Policy`
  * Attack: ``
  * Evidence: `script-src 'self' *.googletagmanager.com *.google-analytics.com c.bing.com *.clarity.ms 'nonce-cUIvUSpym2t9zm0UJrODfdwdHxctCBuX/lnQJAZ3RVw='; style-src 'self' rsms.me 'nonce-cUIvUSpym2t9zm0UJrODfdwdHxctCBuX/lnQJAZ3RVw='; block-all-mixed-content; upgrade-insecure-requests; base-uri 'self'; default-src 'self'; font-src 'self' res-1.cdn.office.net rsms.me; connect-src 'self' *.googletagmanager.com *.google-analytics.com *.analytics.google.com c.bing.com *.clarity.ms`
  * Other Info: `The directive(s): frame-ancestors, form-action is/are among the directives that do not fallback to default-src.`
* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: `Content-Security-Policy`
  * Attack: ``
  * Evidence: `script-src 'self' *.googletagmanager.com *.google-analytics.com c.bing.com *.clarity.ms 'nonce-oRPzt0LvmH1n1fuKyDVRspPiQNdRr42sa+6zZyR3w6Y='; style-src 'self' rsms.me 'nonce-oRPzt0LvmH1n1fuKyDVRspPiQNdRr42sa+6zZyR3w6Y='; block-all-mixed-content; upgrade-insecure-requests; base-uri 'self'; default-src 'self'; font-src 'self' res-1.cdn.office.net rsms.me; connect-src 'self' *.googletagmanager.com *.google-analytics.com *.analytics.google.com c.bing.com *.clarity.ms`
  * Other Info: `The directive(s): frame-ancestors, form-action is/are among the directives that do not fallback to default-src.`
* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location`
  * Method: `GET`
  * Parameter: `Content-Security-Policy`
  * Attack: ``
  * Evidence: `script-src 'self' *.googletagmanager.com *.google-analytics.com c.bing.com *.clarity.ms 'nonce-kAVkqHBuVzbc13BRsnGHLt/mEOtT/Fl4p71lfgRSj6U='; style-src 'self' rsms.me 'nonce-kAVkqHBuVzbc13BRsnGHLt/mEOtT/Fl4p71lfgRSj6U='; block-all-mixed-content; upgrade-insecure-requests; base-uri 'self'; default-src 'self'; font-src 'self' res-1.cdn.office.net rsms.me; connect-src 'self' *.googletagmanager.com *.google-analytics.com *.analytics.google.com c.bing.com *.clarity.ms`
  * Other Info: `The directive(s): frame-ancestors, form-action is/are among the directives that do not fallback to default-src.`
* URL: https://s265d01-app-web.azurewebsites.net/robots.txt
  * Node Name: `https://s265d01-app-web.azurewebsites.net/robots.txt`
  * Method: `GET`
  * Parameter: `Content-Security-Policy`
  * Attack: ``
  * Evidence: `script-src 'self' *.googletagmanager.com *.google-analytics.com c.bing.com *.clarity.ms 'nonce-elK6LkBfaRdoHh8gJYNJhLive8zrF4kolq0tSYDDA8k='; style-src 'self' rsms.me 'nonce-elK6LkBfaRdoHh8gJYNJhLive8zrF4kolq0tSYDDA8k='; block-all-mixed-content; upgrade-insecure-requests; base-uri 'self'; default-src 'self'; font-src 'self' res-1.cdn.office.net rsms.me; connect-src 'self' *.googletagmanager.com *.google-analytics.com *.analytics.google.com c.bing.com *.clarity.ms`
  * Other Info: `The directive(s): frame-ancestors, form-action is/are among the directives that do not fallback to default-src.`
* URL: https://s265d01-app-web.azurewebsites.net/sitemap.xml
  * Node Name: `https://s265d01-app-web.azurewebsites.net/sitemap.xml`
  * Method: `GET`
  * Parameter: `Content-Security-Policy`
  * Attack: ``
  * Evidence: `script-src 'self' *.googletagmanager.com *.google-analytics.com c.bing.com *.clarity.ms 'nonce-bnRJyN4hJuUMTGBtO0j3z6GbMfEmHeNwtNoQ+UGcXBY='; style-src 'self' rsms.me 'nonce-bnRJyN4hJuUMTGBtO0j3z6GbMfEmHeNwtNoQ+UGcXBY='; block-all-mixed-content; upgrade-insecure-requests; base-uri 'self'; default-src 'self'; font-src 'self' res-1.cdn.office.net rsms.me; connect-src 'self' *.googletagmanager.com *.google-analytics.com *.analytics.google.com c.bing.com *.clarity.ms`
  * Other Info: `The directive(s): frame-ancestors, form-action is/are among the directives that do not fallback to default-src.`

Instances: Systemic


### Solution

Ensure that your web server, application server, load balancer, etc. is properly configured to set the Content-Security-Policy header.

### Reference


* [ https://www.w3.org/TR/CSP/ ](https://www.w3.org/TR/CSP/)
* [ https://caniuse.com/#search=content+security+policy ](https://caniuse.com/#search=content+security+policy)
* [ https://content-security-policy.com/ ](https://content-security-policy.com/)
* [ https://github.com/HtmlUnit/htmlunit-csp ](https://github.com/HtmlUnit/htmlunit-csp)
* [ https://web.dev/articles/csp#resource-options ](https://web.dev/articles/csp#resource-options)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ Cookie No HttpOnly Flag ](https://www.zaproxy.org/docs/alerts/10010/)



##### Low (Medium)

### Description

A cookie has been set without the HttpOnly flag, which means that the cookie can be accessed by JavaScript. If a malicious script can be run on this page then the cookie will be accessible and can be transmitted to another site. If this is a session cookie then session hijacking may be possible.

* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `AnalyticsCookie`
  * Attack: ``
  * Evidence: `Set-Cookie: AnalyticsCookie`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `MarketingCookie`
  * Attack: ``
  * Evidence: `Set-Cookie: MarketingCookie`
  * Other Info: ``


Instances: 2

### Solution

Ensure that the HttpOnly flag is set for all cookies.

### Reference


* [ https://owasp.org/www-community/HttpOnly ](https://owasp.org/www-community/HttpOnly)


#### CWE Id: [ 1004 ](https://cwe.mitre.org/data/definitions/1004.html)


#### WASC Id: 13

#### Source ID: 3

### [ Cookie Without Secure Flag ](https://www.zaproxy.org/docs/alerts/10011/)



##### Low (Medium)

### Description

A cookie has been set without the secure flag, which means that the cookie can be accessed via unencrypted connections.

* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies`
  * Method: `GET`
  * Parameter: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Attack: ``
  * Evidence: `Set-Cookie: .AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location`
  * Method: `GET`
  * Parameter: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Attack: ``
  * Evidence: `Set-Cookie: .AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `AnalyticsCookie`
  * Attack: ``
  * Evidence: `Set-Cookie: AnalyticsCookie`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `MarketingCookie`
  * Attack: ``
  * Evidence: `Set-Cookie: MarketingCookie`
  * Other Info: ``


Instances: 4

### Solution

Whenever a cookie contains sensitive information or is a session token, then it should always be passed using an encrypted channel. Ensure that the secure flag is set for cookies containing such sensitive information.

### Reference


* [ https://owasp.org/www-project-web-security-testing-guide/v41/4-Web_Application_Security_Testing/06-Session_Management_Testing/02-Testing_for_Cookies_Attributes.html ](https://owasp.org/www-project-web-security-testing-guide/v41/4-Web_Application_Security_Testing/06-Session_Management_Testing/02-Testing_for_Cookies_Attributes.html)


#### CWE Id: [ 614 ](https://cwe.mitre.org/data/definitions/614.html)


#### WASC Id: 13

#### Source ID: 3

### [ Cookie without SameSite Attribute ](https://www.zaproxy.org/docs/alerts/10054/)



##### Low (Medium)

### Description

A cookie has been set without the SameSite attribute, which means that the cookie can be sent as a result of a 'cross-site' request. The SameSite attribute is an effective counter measure to cross-site request forgery, cross-site script inclusion, and timing attacks.

* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `AnalyticsCookie`
  * Attack: ``
  * Evidence: `Set-Cookie: AnalyticsCookie`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `MarketingCookie`
  * Attack: ``
  * Evidence: `Set-Cookie: MarketingCookie`
  * Other Info: ``


Instances: 2

### Solution

Ensure that the SameSite attribute is set to either 'lax' or ideally 'strict' for all cookies.

### Reference


* [ https://datatracker.ietf.org/doc/html/draft-ietf-httpbis-cookie-same-site ](https://datatracker.ietf.org/doc/html/draft-ietf-httpbis-cookie-same-site)


#### CWE Id: [ 1275 ](https://cwe.mitre.org/data/definitions/1275.html)


#### WASC Id: 13

#### Source ID: 3

### [ Insufficient Site Isolation Against Spectre Vulnerability ](https://www.zaproxy.org/docs/alerts/90004/)



##### Low (Medium)

### Description

Cross-Origin-Resource-Policy header is an opt-in header designed to counter side-channels attacks like Spectre. Resource should be specifically set as shareable amongst different origins.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: `Cross-Origin-Resource-Policy`
  * Attack: ``
  * Evidence: `same-site`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.ico
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.ico`
  * Method: `GET`
  * Parameter: `Cross-Origin-Resource-Policy`
  * Attack: ``
  * Evidence: `same-site`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg`
  * Method: `GET`
  * Parameter: `Cross-Origin-Resource-Policy`
  * Attack: ``
  * Evidence: `same-site`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/govuk-icon-180.png
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/govuk-icon-180.png`
  * Method: `GET`
  * Parameter: `Cross-Origin-Resource-Policy`
  * Attack: ``
  * Evidence: `same-site`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/govuk-icon-mask.svg
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/govuk-icon-mask.svg`
  * Method: `GET`
  * Parameter: `Cross-Origin-Resource-Policy`
  * Attack: ``
  * Evidence: `same-site`
  * Other Info: ``

Instances: Systemic


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

### [ Permissions Policy Header Not Set ](https://www.zaproxy.org/docs/alerts/10063/)



##### Low (Medium)

### Description

Permissions Policy Header is an added layer of security that helps to restrict from unauthorized access or usage of browser/client features by web resources. This policy ensures the user privacy by limiting or specifying the features of the browsers can be used by the web resources. Permissions Policy provides a set of standard HTTP headers that allow website owners to limit which features of browsers can be used by the page such as camera, microphone, location, full screen etc.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/
  * Node Name: `https://s265d01-app-web.azurewebsites.net/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/robots.txt
  * Node Name: `https://s265d01-app-web.azurewebsites.net/robots.txt`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/sitemap.xml
  * Node Name: `https://s265d01-app-web.azurewebsites.net/sitemap.xml`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: Systemic


### Solution

Ensure that your web server, application server, load balancer, etc. is configured to set the Permissions-Policy header.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Permissions-Policy ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Permissions-Policy)
* [ https://developer.chrome.com/blog/feature-policy/ ](https://developer.chrome.com/blog/feature-policy/)
* [ https://scotthelme.co.uk/a-new-security-header-feature-policy/ ](https://scotthelme.co.uk/a-new-security-header-feature-policy/)
* [ https://w3c.github.io/webappsec-feature-policy/ ](https://w3c.github.io/webappsec-feature-policy/)
* [ https://www.smashingmagazine.com/2018/12/feature-policy/ ](https://www.smashingmagazine.com/2018/12/feature-policy/)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ Private IP Disclosure ](https://www.zaproxy.org/docs/alerts/2/)



##### Low (Medium)

### Description

A private IP (such as 10.x.x.x, 172.x.x.x, 192.168.x.x) or an Amazon EC2 private hostname (for example, ip-10-0-56-78) has been found in the HTTP response body. This information might be helpful for further attacks targeting internal systems.

* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `10.02.68.86`
  * Other Info: `10.02.68.86
`


Instances: 1

### Solution

Remove the private IP address from the HTTP response body. For comments, use JSP/ASP/PHP comment instead of HTML/JavaScript comment which can be seen by client browsers.

### Reference


* [ https://datatracker.ietf.org/doc/html/rfc1918 ](https://datatracker.ietf.org/doc/html/rfc1918)


#### CWE Id: [ 497 ](https://cwe.mitre.org/data/definitions/497.html)


#### WASC Id: 13

#### Source ID: 3

### [ Strict-Transport-Security Header Not Set ](https://www.zaproxy.org/docs/alerts/10035/)



##### Low (High)

### Description

HTTP Strict Transport Security (HSTS) is a web security policy mechanism whereby a web server declares that complying user agents (such as a web browser) are to interact with it using only secure HTTPS connections (i.e. HTTP layered over TLS/SSL). HSTS is an IETF standards track protocol and is specified in RFC 6797.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.ico
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.ico`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/robots.txt
  * Node Name: `https://s265d01-app-web.azurewebsites.net/robots.txt`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/sitemap.xml
  * Node Name: `https://s265d01-app-web.azurewebsites.net/sitemap.xml`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: Systemic


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

### [ Cookie Poisoning ](https://www.zaproxy.org/docs/alerts/10029/)



##### Informational (Low)

### Description

This check looks at user-supplied input in query string parameters and POST data to identify where cookie parameters might be controlled. This is called a cookie poisoning attack, and becomes exploitable when an attacker can manipulate the cookie in various ways. In some cases this will not be exploitable, however, allowing URL parameters to set cookie values is generally considered a bug.

* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `AnalyticsCookie`
  * Attack: ``
  * Evidence: ``
  * Other Info: `An attacker may be able to poison cookie values through POST parameters. To test if this is a more serious issue, you should try resending that request as a GET, with the POST parameter included as a query string parameter. For example: https://nottrusted.com/page?value=maliciousInput.

This was identified at:

https://s265d01-app-web.azurewebsites.net/linkpages/cookies

User-input was found in the following cookie:
AnalyticsCookie=yes; expires=Thu, 14 Jan 2027 10:17:38 GMT; path=/

The user input was:
AnalyticsCookie=yes`
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies ()(AnalyticsCookie,MarketingCookie,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `MarketingCookie`
  * Attack: ``
  * Evidence: ``
  * Other Info: `An attacker may be able to poison cookie values through POST parameters. To test if this is a more serious issue, you should try resending that request as a GET, with the POST parameter included as a query string parameter. For example: https://nottrusted.com/page?value=maliciousInput.

This was identified at:

https://s265d01-app-web.azurewebsites.net/linkpages/cookies

User-input was found in the following cookie:
AnalyticsCookie=yes; expires=Thu, 14 Jan 2027 10:17:38 GMT; path=/

The user input was:
MarketingCookie=yes`


Instances: 2

### Solution

Do not allow user input to control cookie names and values. If some query string parameters must be set in cookie values, be sure to filter out semicolon's that can serve as name/value pair delimiters.

### Reference


* [ https://en.wikipedia.org/wiki/HTTP_cookie ](https://en.wikipedia.org/wiki/HTTP_cookie)
* [ https://cwe.mitre.org/data/definitions/565.html ](https://cwe.mitre.org/data/definitions/565.html)


#### CWE Id: [ 565 ](https://cwe.mitre.org/data/definitions/565.html)


#### WASC Id: 20

#### Source ID: 3

### [ Information Disclosure - Suspicious Comments ](https://www.zaproxy.org/docs/alerts/10027/)



##### Informational (Low)

### Description

The response appears to contain suspicious comments which may help an attacker.

* URL: https://s265d01-app-web.azurewebsites.net/js/accessible-autocomplete.min.js
  * Node Name: `https://s265d01-app-web.azurewebsites.net/js/accessible-autocomplete.min.js`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `query`
  * Other Info: `The following pattern was used: \bQUERY\b and was detected in likely comment: "//github.com/zloirock/core-js/blob/v3.36.0/LICENSE",source:"https://github.com/zloirock/core-js"})},4696:function(t,e,n){var r=n", see evidence field for the suspicious comment/snippet.`
* URL: https://s265d01-app-web.azurewebsites.net/js/moj-frontend.min.js
  * Node Name: `https://s265d01-app-web.azurewebsites.net/js/moj-frontend.min.js`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `user`
  * Other Info: `The following pattern was used: \bUSER\b and was detected in likely comment: "//www.w3.org/2000/svg">\n         <path d="M5.5 0L11 5L0 5L5.5 0Z" fill="currentColor"/>\n       </svg>\n      </span>\n    </bu", see evidence field for the suspicious comment/snippet.`


Instances: 2

### Solution

Remove all comments that return information that may help an attacker and fix any underlying problems they refer to.

### Reference



#### CWE Id: [ 615 ](https://cwe.mitre.org/data/definitions/615.html)


#### WASC Id: 13

#### Source ID: 3

### [ Modern Web Application ](https://www.zaproxy.org/docs/alerts/10109/)



##### Informational (Medium)

### Description

The application appears to be a modern web application. If you need to explore it automatically then the Ajax Spider may well be more effective than the standard one.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<noscript><iframe src="https://www.googletagmanager.com/ns.html?id=GA-DEV" height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>`
  * Other Info: `A noScript tag has been found, which is an indication that the application works differently with JavaScript enabled compared to when it is not.`
* URL: https://s265d01-app-web.azurewebsites.net/
  * Node Name: `https://s265d01-app-web.azurewebsites.net/`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<noscript><iframe src="https://www.googletagmanager.com/ns.html?id=GA-DEV" height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>`
  * Other Info: `A noScript tag has been found, which is an indication that the application works differently with JavaScript enabled compared to when it is not.`
* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<noscript><iframe src="https://www.googletagmanager.com/ns.html?id=GA-DEV" height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>`
  * Other Info: `A noScript tag has been found, which is an indication that the application works differently with JavaScript enabled compared to when it is not.`
* URL: https://s265d01-app-web.azurewebsites.net/robots.txt
  * Node Name: `https://s265d01-app-web.azurewebsites.net/robots.txt`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<noscript><iframe src="https://www.googletagmanager.com/ns.html?id=GA-DEV" height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>`
  * Other Info: `A noScript tag has been found, which is an indication that the application works differently with JavaScript enabled compared to when it is not.`
* URL: https://s265d01-app-web.azurewebsites.net/sitemap.xml
  * Node Name: `https://s265d01-app-web.azurewebsites.net/sitemap.xml`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<noscript><iframe src="https://www.googletagmanager.com/ns.html?id=GA-DEV" height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>`
  * Other Info: `A noScript tag has been found, which is an indication that the application works differently with JavaScript enabled compared to when it is not.`

Instances: Systemic


### Solution

This is an informational alert and so no changes are required.

### Reference




#### Source ID: 3

### [ Non-Storable Content ](https://www.zaproxy.org/docs/alerts/10049/)



##### Informational (Medium)

### Description

The response contents are not storable by caching components such as proxy servers. If the response does not contain sensitive, personal or user-specific information, it may benefit from being stored and cached, to improve performance.

* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `no-store`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `no-store`
  * Other Info: ``


Instances: 2

### Solution

The content may be marked as storable by ensuring that the following conditions are satisfied:
The request method must be understood by the cache and defined as being cacheable ("GET", "HEAD", and "POST" are currently defined as cacheable)
The response status code must be understood by the cache (one of the 1XX, 2XX, 3XX, 4XX, or 5XX response classes are generally understood)
The "no-store" cache directive must not appear in the request or response header fields
For caching by "shared" caches such as "proxy" caches, the "private" response directive must not appear in the response
For caching by "shared" caches such as "proxy" caches, the "Authorization" header field must not appear in the request, unless the response explicitly allows it (using one of the "must-revalidate", "public", or "s-maxage" Cache-Control response directives)
In addition to the conditions above, at least one of the following conditions must also be satisfied by the response:
It must contain an "Expires" header field
It must contain a "max-age" response directive
For "shared" caches such as "proxy" caches, it must contain a "s-maxage" response directive
It must contain a "Cache Control Extension" that allows it to be cached
It must have a status code that is defined as cacheable by default (200, 203, 204, 206, 300, 301, 404, 405, 410, 414, 501).

### Reference


* [ https://datatracker.ietf.org/doc/html/rfc7234 ](https://datatracker.ietf.org/doc/html/rfc7234)
* [ https://datatracker.ietf.org/doc/html/rfc7231 ](https://datatracker.ietf.org/doc/html/rfc7231)
* [ https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html ](https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html)


#### CWE Id: [ 524 ](https://cwe.mitre.org/data/definitions/524.html)


#### WASC Id: 13

#### Source ID: 3

### [ Re-examine Cache-control Directives ](https://www.zaproxy.org/docs/alerts/10015/)



##### Informational (Low)

### Description

The cache-control header has not been set properly or is missing, allowing the browser and proxies to cache content. For static assets like css, js, or image files this might be intended, however, the resources should be reviewed to ensure that no sensitive content will be cached.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: `no-cache,no-store`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/
  * Node Name: `https://s265d01-app-web.azurewebsites.net/`
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/manifest.json
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/manifest.json`
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies`
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: `no-cache, no-store`
  * Other Info: ``
* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location`
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: `no-cache, no-store`
  * Other Info: ``

Instances: Systemic


### Solution

For secure content, ensure the cache-control HTTP header is set with "no-cache, no-store, must-revalidate". If an asset should be cached consider setting the directives "public, max-age, immutable".

### Reference


* [ https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#web-content-caching ](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#web-content-caching)
* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cache-Control ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cache-Control)
* [ https://grayduck.mn/2021/09/13/cache-control-recommendations/ ](https://grayduck.mn/2021/09/13/cache-control-recommendations/)


#### CWE Id: [ 525 ](https://cwe.mitre.org/data/definitions/525.html)


#### WASC Id: 13

#### Source ID: 3

### [ Session Management Response Identified ](https://www.zaproxy.org/docs/alerts/10112/)



##### Informational (Medium)

### Description

The given response has been identified as containing a session management token. The 'Other Info' field contains a set of header tokens that can be used in the Header Based Session Management Method. If the request is in a context which has a Session Management Method set to "Auto-Detect" then this rule will change the session management to use the tokens identified.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: `.AspNetCore.Session`
  * Attack: ``
  * Evidence: `.AspNetCore.Session`
  * Other Info: `cookie:.AspNetCore.Session`
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies`
  * Method: `GET`
  * Parameter: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Attack: ``
  * Evidence: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Other Info: `cookie:.AspNetCore.Antiforgery.VyLW6ORzMgk`
* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location`
  * Method: `GET`
  * Parameter: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Attack: ``
  * Evidence: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Other Info: `cookie:.AspNetCore.Antiforgery.VyLW6ORzMgk`
* URL: https://s265d01-app-web.azurewebsites.net/linkpages/cookies
  * Node Name: `https://s265d01-app-web.azurewebsites.net/linkpages/cookies`
  * Method: `GET`
  * Parameter: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Attack: ``
  * Evidence: `.AspNetCore.Antiforgery.VyLW6ORzMgk`
  * Other Info: `cookie:.AspNetCore.Antiforgery.VyLW6ORzMgk`


Instances: 4

### Solution

This is an informational alert rather than a vulnerability and so there is nothing to fix.

### Reference


* [ https://www.zaproxy.org/docs/desktop/addons/authentication-helper/session-mgmt-id/ ](https://www.zaproxy.org/docs/desktop/addons/authentication-helper/session-mgmt-id/)



#### Source ID: 3

### [ Storable and Cacheable Content ](https://www.zaproxy.org/docs/alerts/10049/)



##### Informational (Medium)

### Description

The response contents are storable by caching components such as proxy servers, and may be retrieved directly from the cache, rather than from the origin server by the caching servers, in response to similar requests from other users. If the response data is sensitive, personal or user-specific, this may result in sensitive information being leaked. In some cases, this may even result in a user gaining complete control of the session of another user, depending on the configuration of the caching components in use in their environment. This is primarily an issue where "shared" caching servers such as "proxy" caches are configured on the local network. This configuration is typically found in corporate or educational environments, for instance.

* URL: https://s265d01-app-web.azurewebsites.net
  * Node Name: `https://s265d01-app-web.azurewebsites.net`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `The response is stale, and stale responses are not configured to be re-validated or blocked, using the 'must-revalidate', 'proxy-revalidate', 's-maxage', or 'max-age' response 'Cache-Control' directives.`
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.ico
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.ico`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg
  * Node Name: `https://s265d01-app-web.azurewebsites.net/assets/rebrand/images/favicon.svg`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://s265d01-app-web.azurewebsites.net/robots.txt
  * Node Name: `https://s265d01-app-web.azurewebsites.net/robots.txt`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://s265d01-app-web.azurewebsites.net/sitemap.xml
  * Node Name: `https://s265d01-app-web.azurewebsites.net/sitemap.xml`
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`

Instances: Systemic


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

### [ User Controllable HTML Element Attribute (Potential XSS) ](https://www.zaproxy.org/docs/alerts/10031/)



##### Informational (Low)

### Description

This check looks at user-supplied input in query string parameters and POST data to identify where certain HTML attribute values might be controlled. This provides hot-spot detection for XSS (cross-site scripting) that will require further review by a security analyst to determine exploitability.

* URL: https://s265d01-app-web.azurewebsites.net/location
  * Node Name: `https://s265d01-app-web.azurewebsites.net/location ()(Distance,__RequestVerificationToken)`
  * Method: `POST`
  * Parameter: `Distance`
  * Attack: ``
  * Evidence: ``
  * Other Info: `User-controlled HTML attribute values were found. Try injecting special characters to see if XSS might be possible. The page at the following URL:

https://s265d01-app-web.azurewebsites.net/location

appears to include user input in:
a(n) [input] tag [value] attribute

The user input found was:
Distance=Two

The user-controlled value was:
two`


Instances: 1

### Solution

Validate all input and sanitize output it before writing to any HTML attributes.

### Reference


* [ https://cheatsheetseries.owasp.org/cheatsheets/Input_Validation_Cheat_Sheet.html ](https://cheatsheetseries.owasp.org/cheatsheets/Input_Validation_Cheat_Sheet.html)


#### CWE Id: [ 20 ](https://cwe.mitre.org/data/definitions/20.html)


#### WASC Id: 20

#### Source ID: 3


