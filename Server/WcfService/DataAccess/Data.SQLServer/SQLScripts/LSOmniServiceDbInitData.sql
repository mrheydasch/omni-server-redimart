
delete from [dbo].[AppSettings]
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Cache_Image_DurationInMinutes', N'en', N'525600', N'Image Cache Duration in minutes, 0 is no cache', N'int')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Cache_Menu_DurationInMinutes', N'en', N'0', N'Loyalty Menu Cache Duration in minutes, 0 is no cache ', N'int')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'ContactUs', N'en', N' <p>LS Retail is the leading provider of end-to-end solutions and services for the Retail, Hospitality and Forecourt industries based on Microsoft Dynamics and .NET technology.</p>
<a href="tel:+3544145700">Call us at:</a>
<a href="tel:+3544145708">Support phone number</a>
<a href="mail:info@lsr.com">Email us</a>
<a href="url:www.lsretail.com">Visit our web site</a>', N'the Contact us text for mobile device', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Currency_Code', N'en', N'GBP', N'Local currency code. LSOmni uses this currency code - not getting it from NAV', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Currency_Culture', N'en', N'', N'Ex: en-US  de-DE. By default the UI region on server decides but can be overwritten here', N'string')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'PDF_Save_FolderName', N'en', N'', N'POS Only, FolderName to save PDF slips. Ex: c:\LSOmni\PDF. Empty means it uses \PDF folder under \Service.', N'string')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Demo_Print_Enabled', N'en', N'false', N'true/false,  true then print receipt in NAV will not be called', N'bool')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Forgotpassword_email_body', N'en', N'Did you request a password reset for your LS Retail loyalty account? [CRLF][CRLF]If you requested this password reset, go here: [URL] [CRLF][CRLF]Thanks,[CRLF]The LS Retail account team', N'Email body for the forget password. Variable: [URL]', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Forgotpassword_email_subject', N'en', N'LS Retail loyalty account password reset', N'Email subject line for the forget password', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Forgotpassword_device_email_body', N'en', N'Did you request a password reset for your LS Retail loyalty account? [CRLF][CRLF]Please enter this reset code on your phone: [RESETCODE] [CRLF][CRLF]Thanks,[CRLF]The LS Retail account team', N'Email body for the forget password - for devices. HTML supported. Variable: [RESETCODE]', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Forgotpassword_device_email_subject', N'en', N'LS Retail loyalty account password reset', N'Email subject line for the forget password -for devices', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Forgotpassword_email_url', N'en', N'http://localhost/loyalty.web/ForgotPassword/Resetpassword', N'url for the forget password', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Forgotpassword_code_encrypted', N'en', N'false', N'Reset Code is Encrypted', N'bool')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Forgotpassword_omni_sendemail', N'en', N'true', N'Omni will send out Reset Password Email', N'bool')
GO


INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) VALUES (N'Image_Save_AbsolutePath', N'en', N'', N'The URL where the images are located. Ex http://myserver.com/OmniImages/  Empty means it uses the ApplicationVirtualPath on local machine, http://localhost/LSOmniService/Images', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) VALUES (N'Image_Save_FolderName', N'en', N'', N'FolderName to save images. Ex: c:\LSOmni\Images. Empty means it uses \Images folder under \Service.', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Loyalty_FilterOnStore', N'en', N'S0001', N'Store Id to filter Items', N'string')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Resetpin_email_body', N'en', N'Your PIN has been reset on your lsretail loyalty card. [CRLF][CRLF]Thank you,[CRLF]The ls retail account team', N'Email body for reset pin. HTML supported.', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Resetpin_email_subject', N'en', N'Your PIN has been reset', N'Email subject line for reset pin', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Security_BasicAuth_Pwd', N'en', N'XYZ.LS@mniServiceUser.321', N'BasicAuth password (lsomni way). Used when IIS is anonymous', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Security_BasicAuth_UserName', N'en', N'LSOmniServiceUser', N'BasicAuth user name (lsomni way). Used when IIS is anonymous', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Security_BasicAuth_Validation', N'en', N'false', N'Boolean, true/false, BasicAuth validation (lsomni way). Used when IIS is anonymous, simple check so unknown clients not allowed in  ', N'bool')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Security_Validatetoken', N'en', N'true', N'Boolean, true/false  True the SecurityToken is validated', N'bool')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'TenderType_Mapping', N'en', N'0=1,1=3,2=10,3=11', N'Mapping between Omni and Nav TenderTypeIds.  LSOmniTenderTypeId=NavTenderTypeId,  for ex. cash is 1=1, GiftCard is 10=10 etc.  See enum TenderType ', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'TermsOfService', N'en', N'<h1>Welcome</h1> and more text bla bli bla ', N'the terms of service for iPhone ?  maybe', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Timezone_HoursOffset_DD', N'en', N'0', N'NAV stores everything in UTC, Data Director replicated data may need to use time offset', N'int')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Timezone_HoursOffset_WS', N'en', N'0', N'NAV stores everything in UTC, web service data may need to use time offset', N'int')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Timezone_DayOfWeekOffset', N'en', N'1', N'NAV starts with Sunday as 1 but .Net Sunday=0.  If value is 1 (default) then 1 is subtracted from the DayOfWeek enum', N'int')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'POS_System_Inventory', N'en', N'false', N'Show Inventory button on App', N'bool')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'POS_System_Inventory_Lookup', N'en', N'true', N'Do Inventory lookup for items', N'bool')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Password_Policy', N'en', N'5-character minimum; case sensitive', N'Password policy, enforced in NAV', N'string')
--  5-character minimum; a digit; upper-case; a special character; case sensitive
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'LSNAV_Version', N'en', N'9.00.03', N'LS Nav version used', N'string')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Receipt_Email_Send_From_BO', N'en', N'false', N'true/false, true then receipt is emailed from backoffice (NAV), otherwise LS Omni server will send the email', N'bool')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Receipt_Email_Subject', N'en', N'Receipt [RECEIPTNUMBER] from LS Retail', N'Email subject line for receipt. Variable: [RECEIPTNUMBER]', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Receipt_Email_Body', N'en', N'<html><body> <p/> <pre>[RECEIPT]</pre> <br/> </body></html>', N'Email body for receipt. HTML supported. Variable: [RECEIPT]', N'string')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Registration_Email_Subject', N'en', N'LS Retail registration details', N'Email subject line for registration.', N'string')
GO
INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Registration_Email_Body', N'en', N'<html><body> <p/>Hi [NAME]<p/>This message is an automatic reply to your registration request. 
<br/>Username: [LOGIN] <p/>With this Username you can sign in to our loyalty apps<p/>
Thank you for registering with LS Retail<p/> </body></html>', N'Email body for registration. HTML supported. Variable: [NAME]  [LOGIN]', N'string')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'Use_LSOne_Email', N'en', N'true', N'Indicates wether the server uses local settings or LS One email settings', N'bool')
GO

INSERT [dbo].[AppSettings] ([Key], [LanguageCode], [Value], [Comment], [DataType]) 
VALUES (N'URL_Displayed_On_Client', N'en', N'http://www.lsretail.com', N'A url displayed on client, app or eCommerce', N'string')
