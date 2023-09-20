# Contributing

This repository is specifically for Scroll Snaps for Unity. If you have a
beautiful UI Component, but it doesn't have to do with Scroll Snaps consider
contributing to the [Unity UI Extensions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/overview) project.

##Creative Tenets

1. Components should be as simple looking as possible, the user should only
	need to interact with one monobehavior, even if more is going on behind the
	scenes.
2. Components should not rely on vanilla unity components like Scroll Rects,
	they should handle whatever they need to do themselves, to make it easier
	for the user.
3. What you see is what you get, when the user is organizing items in
	the editor it should look the same as how it will look when they run it.

##Style Guide
Code from Version 1.0.0 and forward follows a simple style guide:

* Any new code files must be prefixed with the [MIT License](https://opensource.org/licenses/MIT). We won't mix licenses.
* Unused "using" statements are not allowed.
* Lines wrap at 100 characters.
* Lines are indented with four spaces.
* Wrapped lines are indented by an extra four spaces.
* Function names are PascalCased.
* Public variables are camelCased.
* Private variables are prefixed m_ and then PascalCased. e.g. m_PrivateVariable.
* Comments are placed on a separate line, not at the end of a line.
* Comments begin with an uppercase letter.
* Comments end with a period.
* Comments have one space between the comment delimeter (//) and the comment text.
* Comments always use (//) they do not use astrisks. The only exception is
	license text.
	
This will not be perfectly consistent, for foolish consistency is the
hobgoblin of little minds, but it is something to strive for.
	
##Active Versions
All pull requests should be made against the latest Version 0 release or the
latest Version 1 release. In-between versions are not supported.
	
## Code Reviews
All submissions require review. We use bitbucket pull requests for this purpose.
After the code has been reviewed pull requests will be merged by BeksOmega.