Feature: Test GraphQL Endpoint with Karate

Background: Base URL and Schemas
  * url baseUrl

  * text userSchema =
    """
      {
        id: '#string',
        name: '#string',
        email: '#string',
        age: '#number',
        birthday: '#string',
        address: '##addressSchema',
        friends: '##[] #userSchema'
      }
    """

  * text addressSchema =
    """
      {
        street: '#string',
        city: '#string',
        state: '#string',
        zip: '#string',
        url: '##string'
      }
    """

  * text articleSchema =
    """
      {
        id: '#string',
        title: '#string',
        content: '#string'
      }
    """

  * text nodeSchema =
    """
      {
        id: '#string'
      }
    """

Scenario: Perform a user query and validate the response
  * text query =
    """
      query UserTest($id: ID!) {
        user(id: $id) {
          id
          name
          email
          age
          birthday
          address {
            street
            city
            state
            zip
            url
          }
        }
      }
    """

  * text variables =
    """
      {
        "id": "4e665b78d44b49ec809165a4359287af"
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "UserTest", variables: variables }
  When method post
  Then status 200
  And match response.data.user == userSchema

Scenario: Perform a users query and validate the response
  * text query =
    """
      query UsersTest($filter: UserFilterInput) {
        users(filter: $filter) {
          id
          name
          email
          age
          birthday
          address {
            street
            city
            state
            zip
            url
          }
        }
      }
    """

  * text variables =
    """
      {
        "filter": { "nameContains": "2d5ba4c0c0ae4191a10f76ecc9a8e4b8", "emailContains": "cf0628e442654e53982cdab411540919", "ageGreaterThan": 499, "ageLessThan": 636, "address": { "streetContains": "18e75caa3b6d4d82984301263dba992a", "cityContains": "9ede382df97146cca9d2883661e88be2", "stateEquals": "19f6b1d0a615449d8317756df7da5c7b", "zipEquals": "e1d829c6231e4ce1ad7dfc0f5e6c5df9", "url": "1199f75459014c9f8e4ac2c9633e98e4" } }
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "UsersTest", variables: variables }
  When method post
  Then status 200
  And match each response.data.users == userSchema

Scenario: Perform a search query and validate the response
  * text query =
    """
      query SearchTest($query: String!, $limit: Int, $offset: Int) {
        search(query: $query) {
          ... on User {
            id
            name
            email
            age
            birthday
            address {
              street
              city
              state
              zip
              url
            }
            friends(limit: $limit, offset: $offset) {
              id
              name
              email
              age
              birthday
              address {
                street
                city
                state
                zip
                url
              }
            }
          }
          ... on Article {
            id
            title
            content
          }
        }
      }
    """

  * text variables =
    """
      {
        "query": "e58394d89e9c440a90c5c05a5dba0850",
        "limit": 560,
        "offset": 649
      }
    """

  * def isValid =
    """
    response =>
      karate.match(response, userSchema).pass ||
      karate.match(response, articleSchema).pass
    """

  Given path "/graphql"
  And request { query: query, operationName: "SearchTest", variables: variables }
  When method post
  Then status 200
  And match each response.data.search == "#? isValid(_)"

Scenario: Perform a article query and validate the response
  * text query =
    """
      query ArticleTest($id: ID!) {
        article(id: $id) {
          id
          title
          content
        }
      }
    """

  * text variables =
    """
      {
        "id": "9fd6fd62bd5d4f08bc83008ae6df86c3"
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "ArticleTest", variables: variables }
  When method post
  Then status 200
  And match response.data.article == articleSchema

Scenario: Perform a articles query and validate the response
  * text query =
    """
      query ArticlesTest($input: ArticleInput) {
        articles(input: $input) {
          id
          title
          content
        }
      }
    """

  * text variables =
    """
      {
        "input": { "titleContains": "ba586d7560b0412fa23348c5780eac55", "contentContains": "b707097235364a29870080badbfdaee3", "authorId": "7c26901aae9f40a9ba5ee5d345119628" }
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "ArticlesTest", variables: variables }
  When method post
  Then status 200
  And match each response.data.articles == articleSchema