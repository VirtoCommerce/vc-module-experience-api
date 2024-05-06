# Getting started
The project "Experience API" it is primarily a intermediated layer between clients and enterprise  services powered by GraphQL protocol and is tightly coupled to a specific user/touchpoint  experience with fast and reliable access, it represents an implementation of Backend for Frontend design pattern (BFF).
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
    },
    "OrderFullTextSearchEnabled": true
}
```

1. The settings “Store full object in index modules” are enabled for catalog and pricing modules:
    ![image](https://user-images.githubusercontent.com/7566324/82232622-29adf380-992f-11ea-8df6-9d08fb0b421a.png)
    ![image](https://user-images.githubusercontent.com/7566324/82232762-5530de00-992f-11ea-8c8c-22766f8fa121.png)

2. Rebuild index

## Test environment

- Deploy `vc-module-experience-api` module into the platform of 3.0 version or latest, guided by this article https://github.com/VirtoCommerce/vc-platform/blob/master/docs/developer-guide/deploy-module-from-source-code.md
- Restart the platform instance
- Open GraphQL UI playground in the browser `http://{platform url}/ui/playground`

**The sample requests:**

```json
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
