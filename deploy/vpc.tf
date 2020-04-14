data "aws_vpc" "vpc" {
  id = "vpc-cdab70a8"
}

data "aws_subnet_ids" "subnets" {
  vpc_id = "${data.aws_vpc.vpc.id}"
}

resource "aws_security_group" "security_group" {
  vpc_id = "${data.aws_vpc.vpc.id}"
  name   = "${var.repo_name}-${var.branch_name}"
  tags   = "${local.altered_tags}"
}
