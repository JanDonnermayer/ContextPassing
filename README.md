# ContextPassing

Demonstrates an approach where customer-data is passed from a merchant website to a shopping-cart-service,
that can be used to correlate the checkout completion.

## Flow

1. The client sends a JSON with required session-information to {cart-api}/funnel-link/{funnelId}
1. The cart-service stores the provided session-data using its hash as key, and returns a link for the checkout-flow including the key in the URL, e.g. checkout/{key}
1. When the URL is hit, the cart-service fetches the stored session-data, and inserts the contained information to the GUI.
1. On checkout completion, the data is sent back to the merchant using webhook. (not part of this demo)

## Infrastructure

- Merchant-Page, Cart-Page, and Cart-API are modeled as a single Function App for simplicity.
- The session-storage is built on Azure Table Storage
