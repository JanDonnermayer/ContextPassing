# Comparison of Approaches to pass Context

## Pass session-data as plain-text in query-parameters to funnel-page

### Pros

- No framework required for merchant

### Cons

- Long url that exposes implementation details
- Url can (accidentally) be modified, which could make it impossible to correlate a successful checkout -> might lose customer


## Pass session-data as JWT token in query-parameter to funnel-page

### Pros

- Security
- Data cannot be tampered with

### Cons

- Very long url
- Merchant needs to use framework and have more expertise
- Secret or public/private-key-pair among other config have to be setup
- Expiration of token


## Pass session-data as JSON to cart-API which creates session and returns funnel-page-link

### Pros

- Short Url
- Data cannot be tampered with
- No framework required for merchant
- Standard JSON format for passing data (no need for encryption since its not exposed)
- Extensible (could be used to pass other options in the future)

### Cons

- Cart-Service needs to provide API and store session-data temporarily
