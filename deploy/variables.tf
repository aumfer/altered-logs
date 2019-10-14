
variable "repo_name" {}

variable "branch_name" {}

variable "ecr_repo" {}

variable "source_rev" {}

variable "aws_region" {
  default = "us-east-1"
}

module "tags" {
  source    = "git::https://github.com/cloudposse/terraform-terraform-label.git?ref=0.1.6"
  name      = "${var.repo_name}-${var.branch_name}"
  namespace = "cai"
  stage     = "${local.stage}"
}

locals {
  altered_tags = {
    repo = "${var.repo_name}"
    env  = "${var.branch_name}"
    sha  = "${var.source_rev}"
  }
}


locals {
  # p  233291844494
  # np 002067833750 
  //stage = "${data.aws_caller_identity.current.account_id == "233291844494" ? "p" : "np"}"
  stage = "np"

  domain_name = "alteredco.com"
}
