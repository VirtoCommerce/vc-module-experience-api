# Filter and facets
This article about base search mechanics of experince api.
## Filters
Search results can optionally be filtered and these filters are applied to the search hits **at the very end of a search request**, after all facets  have already been calculated and don’t influence facet counts any more.

```GraphQL
# The following example search request filters products of a certain color “Black” OR "Blue" AND price between 100 USD inclusive TO 200 USD exlusive AND name starts with "ASUS ZenFone 2"
{
  products(filter: "color:Black,Blue price.usd:[100 TO 200) name:\"ASUS ZenFone 2*\" {
      totalCount
      items
      {
        name
        properties
        {
            name
            values
        }
      }
  }
}
```
### Terms
A query is broken up into terms and operators. There are two types of terms: `Single Terms` and `Phrases`.
A `Single Term` is a single word such as "test" or "hello".
A `Phrase` is a group of words surrounded by double quotes such as "hello dolly".
Multiple terms can be combined together with Boolean operators to form a more complex query (see below).

### Fields
 When performing a search you can either specify a field by typing the field name followed by a colon ":" and then the term you are looking for.

 `name:"My cool name"  color:Black`

Specifying multiple values in one field parameter, separated by a comma, will return products in which at least one of the specified values matches (OR-operator).
The example search request below filters products of the color “black” OR “grey” or "blue":

 `color:Black,Gey,Blue`

### Range Filtration

Range filtration allow one to match products whose field(s) values are between the lower and upper bound specified by the Range expression. Range filter expression can be inclusive or exclusive of the upper and lower bounds. Sorting is done lexicographically.

`price:[100 TO 200]`

This will find products whose prices have values between 100 and 200, inclusive.
Inclusive range queries are denoted by square brackets. Exclusive range queries are denoted by round brackets.

`price:(100 TO 200]`
This will find products whose prices have values between 100 exclusive and 200 inclusive.

You can skip one of the values to ignore either the lower or the upper bound

`price:(TO 100]`
where the price is less than or equal to 100

### Boolean Operators

Passing multiple field terms in the one filter expression separated by space delimiter will combine them with an AND operator.

The following example search request filters products of a certain brand "Onkyo" AND of the color “Black”.
`color:Black brand:Onkyo`

> [!CAUTION]
> At the moment  only AND logical operator is supported for filter expressions.

### Wildcard Searches
You can use single and multiple character wildcard searches within single or phrase terms.
To perform a single character wildcard search use the `?` symbol.
To perform a multiple character wildcard search use the `*` symbol.

`te?t`
Multiple character wildcard searches looks for 0 or more characters.

For example, to search for test, tests or tester, you can use the search:
`test*`

You can also use the wildcard searches in the middle of a term.
`te*t`

### Escaping Special Characters
Inside the double quotes block you might use any unsafe characters, to escape double quote character use the `\`. See more details [GraphQL String-Value](https://spec.graphql.org/June2018/#sec-String-Value)

`\"my cool property\":\"&~!'\"`

### More examples
`color:Black,White`
where the color is 'Black' OR 'White'

`color:Black color:White`
where the color is 'Black' AND 'White'

`price_usd:[100 TO 200]`
where the price is in USD and between values including bounds

`price:(100 TO 200)`
where the price is in any currency and between values excluding bounds

`price:(0 TO)`
where the price is greater than zero

`price:(TO 100]`
where the price is less than or equal to 100

`Da?? Red*`
use ? to replace a single character, and * to replace zero or more characters

`color:Black price:[100 TO 200)`
combine keywords and filters

## Facets
Faceted search (sometimes also called faceted navigation) allows users to navigate through a web site by applying filters for categories, attributes, price ranges and so on. The main idea behind faceted search is to present the attributes of the documents of the previous search result as filters, which can be used by the user to narrow down search results along with calculate statistical counts to aid.

Facet calculation is requested by providing facet expression via the facet query parameter. Consider for example the following two facets:

`facet: "color price:[TO 100),[100 TO 200])"`

The resulting json would be like seen here:

```json
"data": {
    "products": {
      "totalCount": 182,
      "items": [...],
      "range_facets": [
        {
          "name": "price_*-100_100-200",
          "ranges": [
            {
              "from": 0,
              "to": 100,
              "count": 5959,
              "includeTo": false,
              "includeFrom": true
            },
            {
              "from": 100,
              "to": 200,
              "count": 2143,
              "includeTo": true,
              "includeFrom": true
            }
          ]
        }
      ],
      "term_facets": [
        {
          "name": "color",
          "terms": [
            {
              "term": "EXPRESSO",
              "count": 2343
            },
            {
              "term": "Sierra Brown",
              "count": 362
            },
            ...
        }]
    }
}
```

### TermFacet expression
To retrieve facet counts for all occurring values of a product document field the following notations can be applied:

`facet: "category.path"`
Counts the products of all categories.

`facet: "{propertyName}"`
`facet: "properties.{propertyName}"`
Counts the product documents for all occurring values of custom  properties.

### TermFacet result
The term type facets provide the counts for each of the different values the query parameter happens to have.
`name` - represents the key of requested facet taken from facet expression.
`terms.term` - one of the values for the field specified in facet expression for which at least one product could be found
`terms.count` - amount of products  for which the term applies
`terms.isSelected` - flag indicates that requested facet term is used in `filter` expression, in order to simplify displaying the already selected facet terms on the frontend.

### RangeFacet expression
To aggregate facet counts across ranges of values, the range qualifier can be applied analogous to the filter parameters. The `range` notation is applicable to the date, time, datetime, number and money type fields.

`facet: "price.{currency}:[TO 100),[100 TO 200])"`
Counts the products whose price falls in one of the specified ranges

`facet: "properties.{propertyName}[1 TO 100)"`
Counts the products whose values of the custom property fall in one of the specified ranges

### RangeFacet result
The range facet type counts the products for which the query value is a range specified in the range expression. Range facets are typically used to determine the minimum and maximum value for example product prices to filter products by price with a range slider.

`name` - represents the key of requested facet taken from facet expression and build from range parameters concatenated by `_`. e.g `price_*-100_100-200`
`ranges.from` - the range’s lower endpoint in number format
`ranges.to` - the range’s upper endpoint in string format
`ranges.count` - amount of products fall into the specified range
`ranges.includeTo` - flag indicates that lower bound is included
`ranges.includeFrom` - flag indicates that upper bound is included
`ranges.isSelected` - flag indicates that requested facet term is used in `filter` expression, in order to simplify displaying the already selected facet terms on the frontend.
