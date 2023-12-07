# core infra

核心系統的 IaC

## How to build

- set secret in secret manager, each ref for secret can be found [here](./main.tf)
- install cloud build git app in git repository
- set secret in `terraform.tfvars` like `terraform.sample.tfvars`
- `make start` to apply
