# Experience API module
## Functional blocks
“Experience API” project at beginning consists from these three functional blocks :
- [X-Catalog docs](x-catalog-reference.md)
- [X-Purchase cart docs](x-purchase-cart-reference.md)
- [X-Purchase order docs](x-purchase-order-reference.md)
- [X-UserProfile docs](x-profile-reference.md)
- [Recommendations Gateway API](https://github.com/VirtoSolutions/vc-module-experience-api/tree/master/samples/RecommendationsGatewayModule) (prototype)

## Prerequisites
- VC platform 3.0 or later
- The platform is configured to use ElasticSearch engine

**appsettings.json**
```Json
"Search": {
        "Provider": "ElasticSearch",
        "Scope": "default",
        "ElasticSearch": {
            "Server": "localhost:9200",
            "User": "elastic",
            "Key": "",
            "EnableHttpCompression": ""
        }
    },
```
1. The settings “Store full object in index modules” are enabled for catalog and pricing modules:
![image](https://user-images.githubusercontent.com/7566324/82232622-29adf380-992f-11ea-8df6-9d08fb0b421a.png)
![image](https://user-images.githubusercontent.com/7566324/82232762-5530de00-992f-11ea-8c8c-22766f8fa121.png)

1. Rebuild index

## Digital catalog experience API - Getting started.

- Deploy `vc-module-experience-api`  module into the platform of 3.0 version or latest, guided by this article https://github.com/VirtoCommerce/vc-platform/blob/master/docs/developer-guide/deploy-module-from-source-code.md
- Restart the platform instance
- Open GraphQL UI playground in the browser `http://{platform url}/ui/playground`

**The sample requests:**
```js
{
  product(id: "0f7a77cc1b9a46a29f6a159e5cd49ad1")
  {
    id
    name

    properties {
      name
      type
      values
    }
  }

  products(query: "sony" fuzzy: true filter: "price.USD:(400 TO 1000]")
  {
    totalCount
    items {
       name
       id
       prices (currency: "USD") {
        list
        currency
      }
    }
  }
}
```

## Recommendations gateway API - Getting started.
- Install `Digital catalog experience API` see above.
- Install [Recommendations Gateway API module](https://github.com/VirtoSolutions/vc-module-experience-api/tree/master/samples/RecommendationsGatewayModule)
- Add to the platform configuration file `appsettings.json` the following settings from this example https://github.com/VirtoSolutions/vc-module-experience-api/blob/master/samples/RecommendationsGatewayModule/RecommendationsGatewayModule.Core/Configuration/samples/vc-recommendations.json
this settings add vc product association APIs as downstream for  product recommendations.
- Open GraphQL UI playgoround in the browser `http://{platform url}/ui/playground` to start consume API.
The recommendations example GrapQL request:
```Js
  recommendations(scenario:"vc-recommendations" after: "10" first: 5 itemId: "8b7b07c165924a879392f4f51a6f7ce0")
  {
    items
    {
      scenario
      product
      {
        id
        name
      }
    }
  }
```
