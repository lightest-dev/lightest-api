# Access rights API

To be done in 0.11

## Top-level methods for Access API

* CanDiscover - implemented directly in methods for better performance.
* CanRead(Item, User) - can actually read Item details (not Relationships)
* CanAdd(Item, User) - can create Items and add read-only Users to it.
* CanWrite(Item, User) - can modify Items and add read-only Users to it.
* CanChangeAccess(Item, User) - can allow other Users to write items.

## Role Helper

* IsAdmin
* IsTeacher

System Items should use Role Helper instead of Access API.
