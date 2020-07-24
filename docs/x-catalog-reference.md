# X-Digital-Catalog provides high performance search queries for catalog data directly from search index engine

## X-Digital-Catalog key features: 
-	Full-text search
-	Fuzzy search
-	Filters (syntax)
-	Filter by category subtree
-	Filter by price
-	Filter by custom properties
-	Filter by products availability
-	Facets
-	Multi-select faceting search

## Query GraphQL 

### Playground IDE

To explore the GraphQL API, you can use an interactive  [graphql-playground](https://github.com/prisma-labs/graphql-playground) environment.
To open playground console open  `ui/playground` in the platform manager application.
```
http://localhost:10645/ui/playground
```

### Curl

```curl
POST https://{platform-url}/graphql
```

It accepts POST requests with following fields in a JSON body:

- `query` - String - GraphQL query as a string
- `variables` - Object - Optional - containing JSON object that defines variables for your query
- `operationName` - String - Optional - the name of the operation, in case you defined several of them in the query
  
Here is an example of a GraphQL query:

```curl
$ curl -X POST http://localhost:10645/graphql \
  -H "Content-Type:application/json" \
  -H "Authorization:Bearer ..." \
  -d '{"operationName":null,"variables":{},"query":"{ product(id: \"019e93d973cd4adab99b6f9cbb4ca97a\") { name }}"}'
```

## Queries

> Placeholder for automatic data insertion for all x-catalog queries from GraphQL schema

## Full-Text Search

The `query` -  parameter performs the full-text search on the product index document. Expects the full-text search phrase.

### Searchable fields
The search conducts a full-text search over product data in the index. A product search returns all product variations a product has.
We are stored all searchable text data of a product  in one `__content` field in the resulting index document and perform full-text search only for this field.

*product document in the index*
```json
"__content": [
    "JGC-85796278",
    "ASUS ZenFone 2 ZE551ML Gold",
    "asus",
    "android",
    "2.3 ghz intel gtx quad-core",
    "micro-sim",
    "1080"
  ],
```
The following product properties are stored in the `__content` field and are searched by default.

- `product.name`
- `product.code`
- `product.seoinfos.seoinfo.slug`
- `product.properties.value`
- `product.variations.code`
- `product.variations.properties.value`

Example requests:

```GraphQL
# Search all products by keyword `sony` and return the  name and primary image url for first 20 found products and total count
{
  products(query: "sony" first:20) {
      totalCount
      items
      {
        name    
        imgSrc
      }
  }
}
```
## Fuzzy Search

When the `fuzzy` query parameter is set to true the search will also return [product]() that contain slight differences to the search text.
For example, when someone searches for ‘unversty’, the fuzzy search would also return products labelled with ‘university’.
The fuzzy level can be optionally set with the parameter `fuzzyLevel`, otherwise the will be used auto fuzzy level based on the length of the searched text. Min value 3, max value 6.

Example requests:

```GraphQL
# Will return products that have such terms "university", "unversty", "universe" etc.
{
  products(query: "unversty" first:20) {
      totalCount
      items
      {
        name    
        imgSrc
      }
  }
}
```

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


## Filter by category 
Filter products that belong to exactly specified category path.
`filter: "categories.path:{catalog id/category path}"`

`filter: "categories.path:catalogId/cat1d1/cat2id"`

> The search will be performed on `__path` index field of product document

Filter by category subtrees, keep only the products that belong to the specified Category or any of its descendant categories.
`filter: "categories.subtree:{catalog id/category path}"`

`filter: "categories.subtree:catalogId/cat1d1/cat2id"`

> The search will be performed on `__outline` index field of product document


## Filter by price
Keep only the products which Price match the specified value or [range]()

`filter: "price.{currency}.{pricelist?}:{range expression}"`

`filter: "price.usd:(TO 100]"`

`filter: "price.usd.pricelist_1:(20 TO 100]"`

Keep only products that  with at least one price set

`filter: "is:priced`

> The search will be performed on `price_{currency}` and `price_{currency}_{pricelist}` index fields of product document
> Please note, that only the indexed prices were used for filtration. Scoped prices based on user groups or dynamic expressions temporary do not support filtration.

## Filter by SKU
Keep only the product which matches the specified SKU:
`filter: "sku:DLL-65789352`

## Filter products or variations 
Keep only the products or variations in result. If not set will return both types.

`filter: "is:product`

`filter: "is:variation`

## Filter by custom properties
Keep only the products or variation with the custom attribute matching the specified value or range.

`filter: "properties.{property name}: {value}`

`filter: "properties.color:red`

To use property name contains spaces need to use the following syntax with escaped double quotes
`filter= "\"processor core (ghz)\":\"1.8 GHz Intel GTX Quad-Core\""`

For numeric and date time properties you might use range filter

`filter: "length:(10 TO 20)"`

`filter: "publishDate:(TO \"2020-01-28\")"`

> All product custom properties are stored in the index as fields with the same names as properties have.  `{property.name}:{property.value}`

## Filter by product availability  
Keep only the products or variations with the availability matching the specified value or range.

`filter: "available_in:{warehouse}"`

`filter: "available_in:my-warehouse"`

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

## Muti-select faceting search
The policy let select multiple values of the same facet (e.g using checkbox).
You can read more for how the muti-select faceting search work on this great article [How to implement multi-select faceting for nested documents in Solr](https://blog.griddynamics.com/multi-select-faceting-for-nested-documents-in-solr/) and [Elastic search post filter](https://www.elastic.co/guide/en/elasticsearch/reference/7.6/search-request-body.html#request-body-search-post-filter)

The muti-select faceting search policy is enabled by default and you do not need any extra parameters or setting to activate it.
```json
```


## Sorting
By default, search results are sorted descending by their relevancy with respect to the provided text (that is their “score”). An alternative sorting can be specified  via the sort query parameter which has the structure `{field}:{asc|desc}` you can combine multiple sort expression  by semicolon  `;`

`sort: "priority:desc;price.usd;score"`