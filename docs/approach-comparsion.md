# Comparison of Approaches to pass Context

## Pass plain-text query parameters

### Pros

- Most simple to implement for cart-service
- No framework required for merchant

### Cons

- Long url that exposes implementation details
- Url can (accidentally) be modified, making it impossible to correlate a successful checkout -> might lose customer


## Pass a JWT token with data contained as query-parameter to funnel-page

### Pros

- Security
- Data cannot be tampered with

### Cons

- Very long url
- Merchant needs to use framework and have more expertise
- Secret or public/private key pair have to be defined


## Pass json-data to Cart-API that returns checkout-link with unique url

### Pros

- Short Url
- Data cannot be tampered with
- Can use Header for authentication against api
- Simple JSON format for passing data (no need for encryption since its not exposed)
- No framework required for merchant

### Cons

- Cart-Service needs to provide API and store data temporarily
