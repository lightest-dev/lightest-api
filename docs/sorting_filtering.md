# Methods which support sorting and filtering

## Get Categories

URL: /categories  
Supported fields:

* Name
* Public

Future supported fields:

* CanRead
* CanWrite

## Get All Categories (admin only)

URL: /categories/all  
Supported fields:

* Name
* Public

## Get Available Groups

URL: /groups  
Supported fields:

* Name
* Public

Future supported fields:

* CanRead
* CanWrite

## Get All Groups (admin only)

URL: /groups/all  
Supported fields:

* Name
* Public

## Get Tasks (admin only)

URL: /tasks  
Supported fields:

* Name
* Public

## Get users (admin only)

URL: /profile/all  
Supported fields:

* Name
* Surname
* Email
* UserName

## Task uploads (teacher/admin only)

URL: /uploads/{taskId}/all  
Supported fields:

* UserId
* Status
* Points

## Usage

Full docs available here: <https://github.com/Biarity/Sieve/blob/master/README.md>

### Send a request

An example:

```curl
GET /GetPosts
?sorts=     LikeCount,CommentCount,-created         // sort by likes, then comments, then descendingly by date created
&filters=   LikeCount>10, Title@=awesome title,     // filter to posts with more than 10 likes, and a title that contains the phrase "awesome title"
&page=      1                                       // get the first page...
&pageSize=  10                                      // ...which contains 10 posts
```

More formally:

* `sorts` is a comma-delimited ordered list of property names to sort by. Adding a `-` before the name switches to sorting descendingly.
* `filters` is a comma-delimited list of `{Name}{Operator}{Value}` where
    * `{Name}` is the name of a property with the Sieve attribute or the name of a custom filter method for TEntity
        * You can also have multiple names (for OR logic) by enclosing them in brackets and using a pipe delimiter, eg. `(LikeCount|CommentCount)>10` asks if `LikeCount` or `CommentCount` is `>10`
    * `{Operator}` is one of the [Operators](#operators)
    * `{Value}` is the value to use for filtering
        * You can also have multiple values (for OR logic) by using a pipe delimiter, eg. `Title@=new|hot` will return posts with titles that contain the text "`new`" or "`hot`"
* `page` is the number of page to return
* `pageSize` is the number of items returned per page

### Operators

| Operator   | Meaning                  |
|------------|--------------------------|
| `==`       | Equals                   |
| `!=`       | Not equals               |
| `>`        | Greater than             |
| `<`        | Less than                |
| `>=`       | Greater than or equal to |
| `<=`       | Less than or equal to    |
| `@=`       | Contains                 |
| `_=`       | Starts with              |
| `!@=`      | Does not Contains        |
| `!_=`      | Does not Starts with     |
| `@=*`      | Case-insensitive string Contains |
| `_=*`      | Case-insensitive string Starts with |
| `==*`      | Case-insensitive string Equals |
| `!@=*`     | Case-insensitive string does not Contains |
| `!_=*`     | Case-insensitive string does not Starts with |