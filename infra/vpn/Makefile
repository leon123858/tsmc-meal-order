all:

start: 
	terraform init
	terraform plan -var-file="terraform.tfvars"
	terraform apply -var-file="terraform.tfvars"

format:
	terraform fmt -recursive 

init:
	terraform init

plan:
	terraform plan

destroy:
	terraform destroy

connect:
	# bind vpn to cloud run named order
	gcloud run services update order --region asia-east1 --vpc-connector serverless --vpc-egress private-ranges-only

disconnect:
	# unbind vpn to cloud run named order
	gcloud run services update order --region asia-east1 --clear-vpc-connector