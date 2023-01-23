# anngularStore

this is an app that allows you to search the repository of github

## Installation

on the userAPI controller, you need to use your auth token. i have sent one with the email

## explanation

```csharp
class UserapiController{
 Initialize()//used to auth the jwt.
 Login(user,pass,register)//used to login. adds to db if register is true.
 AddToFav(name,url,avatar)// adds the repository to the user identified by jwt
 Search(search)//uses the github api.
}
```
## UI
the UI of he app, is in the UI folder.
when first calling ng serve, you will get a login/register page.
insert admin admin and press login for default, or any (no restrictions) and press register.
once you do, you will be taken to the home page.
at the top, you will see a search bar. the api will be called with every key press (with debounce, to prevent fast calls).
after the search, the results will be shown bellow.
you will then be able to click the bookmark button, to add a repository to favorites. which will be saved to that specific user.
