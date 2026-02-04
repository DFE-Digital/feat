# Find education and training – Accessibility Audit

Auditor - Jake Lloyd - DesignOps DfE  
Date – 07/01/2025

---

## Executive summary

During testing 15 issues were discovered. Of the 15 issues, 4 are high severity, 10 are medium severity and 1 is low severity.

Errors and warnings have been recorded within the report. Errors are Level A and AA WCAG failures, whereas warnings are issues relating to inclusive design, best practice, or Level AAA WCAG issues. To be fully compliant with the WCAG AA standard, all errors will need to be addressed. It is also advised that for the best user experience, all warnings are also addressed.

As a government department our services must be fully compliant with the Public Sector Bodies Accessibility Regulations 2018 (PSBAR). In its current state the service is partially compliant with the WCAG 2.2 AA standard. Resolving the WCAG issues would result in the service being AA compliant.

---

## Test scope

All user journeys provided have been tested. Any issues that have been reported will need to be resolved across any relevant pages.

Testing has been conducted using a range of techniques. This includes testing with assistive technology, automated testing, as well as manual testing at a code level to provide detailed issues, as well as recommendations for resolving issues.

---

## Contents

- Executive summary  
- Test scope  
- Issues  
- Find education and training opportunities page  
- Where do you want to study page  
- What courses, subjects, jobs, or careers are you interested in page  
- What qualification level are you looking for page  
- Your search results page  

---

<!-- PAGE BREAK -->

## Issues

---

## Find education and training opportunities page

### Issue 1: 2.4.2 - Page Titled - Level A - Error

Location: Find education and training opportunities (Occurs across all pages)  
Severity: Medium

The page title for all pages across the service is not unique. When moving to a new page the page title does not change to reflect the contents of the new page.

Screen reader users rely on accurate page titles to understand the context of the page.

Recommendation:  
Update the page title of each page. The page title should contain the H1 of the page followed by the name of the service, you could also then include the prefix of the domain that the service is on. For example:

‘Where do you want to study – Find education and training opportunities – Department for Education’.

---

### Issue 2: Best practice - Warning

Location: Find education and training opportunities  
Severity: Medium

There is no heading level 1 on the initial Find education and training opportunities page.

A heading level 1 provides screen reader users with an understanding of the main topic of the page. Without this it can sometimes be difficult for users to understand the content, and they may continue to navigate the page looking for a H1.

Recommendation:  
Change the Find education and training opportunities heading from a heading level 2 to a heading level 1.

---

### Issue 3: 1.4.4 - Resize Text - Level AA - Error

Location: Find education and training opportunities  
Severity: Low

When using browser settings to resize only the text on the page, not all text is responsive and is able to be resized. The heading and paragraph content do not increase in size.

Some users with low vision or visual impairments use the browser settings to just increase the text size on the page, rather than using zoom. As not all text resizes, this could make it difficult for users to perceive all the content on the page.

Recommendation:  
Use responsive font-size units such as em units to ensure that when browser text size settings are changed, all content is affected. Currently, static px units have been used which are not responsive and will not allow the size to be changed. Change the <p> content and the <h2> content to em units.

NOTE – This occurs across most pages, and the fix should be replicated across all pages.

---

<!-- PAGE BREAK -->

## Where do you want to study page

### Issue 4: Best practice - Warning

Location: Where do you want to study  
Severity: Low

When JavaScript is disabled the input fields are no longer available.

Some users have JavaScript disabled, have a slow or intermittent connection which can affect JavaScript loading, or a device which doesn’t have enough memory to load JavaScript. This would mean they would be unable to complete this page due to the input causing an error which cannot be resolved.

Recommendation:  
Ensure that progressive enhancement is used to build pages and that all pages are usable with JavaScript and CSS turned off. Ensure this is resolved across all pages which use inputs.

---

### Issue 5: Best practice - Warning

Location: Where do you want to study  
Severity: Medium

Both questions on the ‘Where do you want to study’ page are marked up as optional, but when selecting one of the radio buttons from the ‘How far are you able to travel’ question, the ‘Enter a town, city or postcode’ question becomes mandatory.

This could be very confusing, especially of users who have a cognitive disability. Also, if a user accidentally selects one of the radio buttons, they cannot be deselected. This means that users could get stuck on this page if an error is triggered.

Recommendation:  
The page could be reworked so that these questions are split across pages or could use conditional reveal to show the second question only if something has been entered in the first input.

---

### Issue 6: 1.3.1 - Info and Relationships - Level A - Error

Location: Where do you want to study  
Severity: High

The error message for the ‘Enter a town, city or postcode’ question is not associated with the input.

Screen reader users often navigate through webpages by using the tab key. This means that users do not always navigate through noninteractive content. As the error message is not associated with the input, when a screen reader user tabs into the input, the error message is not announced. A users could therefore miss out on the error and be unaware that this input required attention.

Recommendation:  
Add an ID to the <p> element which contains the error message. Also add an aria-described attribute to the <input> element which references the ID used for the <p>.

---

### Issue 7: 3.3.2 - Labels or Instructions - Level A - Error

Location: Where do you want to study  
Severity: Medium

The ‘Enter a town, city or postcode’ input uses an autocomplete feature which provides users with a drop-down list of results. There is not any content to advise users of this functionality or that users also have to select one of these options to continue, they cannot enter their own free text and submit.

Users with cognitive disability or who are neurodivergent could struggle to complete this field. It is not clear to users that when they start typing options will appear that they can select.

Recommendation:  
Add some hint text to this input which explains to users the additional functionality available with this input. This could also include how many characters users must type before autocomplete options appear.

---

<!-- PAGE BREAK -->

## What courses, subjects, jobs, or careers are you interested in page

### Issue 8: 1.3.1 - Info and Relationships - Level A - Error

Location: What courses, subjects, jobs, or careers are you interested in?  
Severity: High

A fieldset has been used to group in the inputs on this page, but a legend has not been used to provide a description of the form.

Screen reader users rely on a legend to provide them with an understanding of what the group of forms relates to.

Recommendation:  
Add a legend to the fieldset which provides an overview of what the user is being asked for. For example, in this case the legend could be ‘Your interests’.

---

<!-- PAGE BREAK -->

## What qualification level are you looking for page

### Issue 9: Best practice - Warning

Location: What qualification level are you looking for?  
Severity: Medium

There are multiple heading level 1’s on this page.

NOTE – This occurs across multiple pages. Ensure that all pages only have one heading level 1.

---

<!-- PAGE BREAK -->

## Your search results page

### Issue 10: 1.3.1 - Info and relationships – Level A - Error

Location: Your search results  
Severity: Medium

The filter on this page users a <dl> element. These elements must only directly contain properly ordered <dt> and <dd> groups, <script>, <template> or <div> elements. Within the filter the <dl> element contains other elements such as buttons which are not permitted within a <dl> element.

Screen readers have a specific way of announcing definition lists. When such lists are not properly marked up, this creates the opportunity for confusing or inaccurate screen reader output.

Recommendation:  
Remove the <dl> element from the filter as it doesn’t seem to be required here.

---

### Issue 10: Best practice - Warning

Location: Your search results  
Severity: Medium

An aria-live attribute has been used on the heading which is displayed if no search results are available.

As this heading only appears when the filter is updated or from an initial search which does not yield results, this is not a live region and does not require users' attention to be taken straight to this content.

The heading can also appear when using the filter, but when selecting a filter option, the page reloads, so a live region is not required here either.

As no new content is being added to the DOM without the page reloading the aria-live roles are unlikely to work here anyway. Role=”status” also already has aria-live=”polite” implicitly so having both of these attributes would be duplication.

Recommendation:  
Remove the role=”status” and aria-live=”polite” attributes from the <h2> element.

---

### Issue 11: 1.3.1 - Info and relationships - Error

Location: Your search results  
Severity: High

The filter on this page uses multiple fieldsets to group options but they do not have a legend to explain the groups.

Screen reader users rely on a legend to provide them with an understanding of what the group of inputs relate to.

Recommendation:  
Add a legend to the fieldset which provides an overview of what the user is being asked for.

---

### Issue 12: 2.4.4 - Link Purpose (In Context) - Level A

Location: Your search results  
Severity: Medium

Within the filter, if multiple filter options have been selected within each section, further ‘clear filter’ links are added to each section. These links are not unique, and all have link text of ‘clear filter’.

If a screen reader user is tabbing through the options within the filter, they would not be aware which section of the filter they are going to clear.

Recommendation:  
Add either visible link text which would be visible for all users, or visually hidden text for just screen readers which provide further information on which section of the filter will be cleared.

---

### Issue 13: Best practice - Warning

Location: Your search results  
Severity: Medium

There are multiple heading level 1’s used on this page.

This can be confusing for screen reader users. Usually, a heading level 1 gives users an understanding of the main page contents. As there are multiple H1’s this confuses the main purpose of the page and could also make users believe they have navigated to a new page.

Recommendation:  
Ensure that there is only one heading level 1 on the page.

---

### Issue 14: Best practice - Warning

Location: Your search results  
Severity: Medium

The filter used on this page is quite long and does not allow users to expand or collapse sections. There is also only one ‘Apply filters’ button.

This means that for keyboard only users there can be a lot of navigation required to get through the filter to navigate to the ‘apply filters’ button or to move to other sections of the filter.

Recommendation:  
The MOJ filter component could be used to replace this filter component and an additional apply filters button could be added to the top of the filter.

---

### Issue 15: 1.4.3 - Contrast (Minimum) - Level AA

Location: Your search results  
Severity: High

The colours used for the foreground and background colours within the pagination do not sufficiently contrast.

The black font colour used for the foreground colour does not contrast with the dark blue background colour. This could make it difficult or impossible for some users with visual impairments or sight loss to perceive the page number.

Ensure that the selected page changes colour to white to contrast with the background colour.
