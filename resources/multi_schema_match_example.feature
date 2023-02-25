Feature: Reference Feature with Multi-Schema Validation

Scenario: Validate Payload
  * def payload = 
    """
      [
        { "id": "one", "favoriteNumber": 223 },
        { "id": 2, name: "will" }
      ]
    """

  * def schema = 
    """
    { 
      "id": "#number",
      "name": "#string" 
    }
    """

  * def otherSchema = 
    """
    { 
      "id": "#string", 
      "favoriteNumber": "#number" 
    }
    """

  * def isValid = 
    """
    response =>
      karate.match(response, schema).pass ||
      karate.match(response, otherSchema).pass
    """
  
  # match each
  * match each payload == "#? isValid(_)"

  # or match individually
  * match payload[0] == "#? isValid(_)"
  * match payload[1] == "#? isValid(_)"
