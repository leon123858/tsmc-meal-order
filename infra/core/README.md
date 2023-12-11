# core infra

核心系統的 IaC

## How to build

- should prepare cloud storage for static and cloud sql for database before (not maintain in this IaC)
    - cloud sql name: tw-rd-ca-leon-lin:asia-east1:meal-dev (postgres sql)
    - cloud storage name: meal-order-static (allow public access ACL, Fine-grained access control)
- set secret in secret manager, each ref for secret can be found [here](./main.tf)
- install cloud build git app in git repository
- set secret in `terraform.tfvars` like `terraform.sample.tfvars`
- `make start` to apply
