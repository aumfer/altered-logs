resource "aws_lambda_function" "cloudwatch" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-cloudwatch"
  role             = "${aws_iam_role.iam.arn}"
  handler          = "Altered.Logs::Altered.Logs.Cloudwatch.LogCloudwatchLogLambda::Execute"
  source_code_hash = "${base64sha256(file("../publish.zip"))}"
  runtime          = "dotnetcore2.1"
  timeout          = "59"                                                                  # srsly amazon should just have a seperate first-run timeout

  tags = "${local.altered_tags}"

  environment {
      variables = "${merge(local.altered_tags, map("ELASTICSEARCH_URL", "https://${aws_elasticsearch_domain.search.endpoint}"))}"
  }
}

resource "aws_lambda_permission" "cloudwatch_allow_events" {
  statement_id  = "${var.repo_name}-${var.branch_name}-cloudwatch-events"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.cloudwatch.function_name}"
  principal     = "logs.${var.aws_region}.amazonaws.com"
}

resource "aws_cloudwatch_log_subscription_filter" "cloudwatch_log_subscription" {
  name            = "${var.repo_name}-${var.branch_name}-cloudwatch"
  destination_arn = "${aws_lambda_function.cloudwatch.arn}"
  log_group_name  = "${data.aws_cloudwatch_log_group.acl.name}"
  filter_pattern  = ""
}
