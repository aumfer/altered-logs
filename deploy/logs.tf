data "aws_cloudwatch_log_group" "acl" {
  name = "acl"

  #tags = "${local.altered_tags}"
}

data "aws_s3_bucket" "elb_logs" {
    bucket = "altered-logs-elb"
}

data "aws_s3_bucket" "alb_logs" {
    bucket = "altered-logs-alb"
}