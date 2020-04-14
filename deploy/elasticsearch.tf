data "aws_iam_policy_document" "es_policy" {
  statement {
    actions = [
      "es:*"
    ]

    resources = [
      "arn:aws:es:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:domain/${var.repo_name}-${var.branch_name}/*",
    ]

    principals {
      type        = "*"
      identifiers = ["*"]
    }
  }
}


resource "aws_elasticsearch_domain" "search" {
  domain_name           = "${var.repo_name}-${var.branch_name}"
  elasticsearch_version = "6.5"

  cluster_config {
    instance_type = "i3.large.elasticsearch"
    #instance_count = "${length(data.aws_subnet_ids.subnets.ids)}"
    instance_count = 1
    dedicated_master_enabled = false
    #dedicated_master_type = "m4.large.elasticsearch"
    #dedicated_master_count = 3
    #zone_awareness_enabled = true
  }

  access_policies = "${data.aws_iam_policy_document.es_policy.json}"

  vpc_options {
    #subnet_ids = ["${data.aws_subnet_ids.subnets.ids}"]
    subnet_ids = ["${data.aws_subnet_ids.subnets.ids[0]}"]

    security_group_ids = ["${aws_security_group.security_group.id}"]
  }

  snapshot_options {
    automated_snapshot_start_hour = 0
  }

  tags = "${local.altered_tags}"

  #lifecycle {
  #  prevent_destroy = true
  #}
}
