resource "aws_s3_bucket_notification" "elb_log_notify" {
  bucket = "${data.aws_s3_bucket.elb_logs.id}"

  lambda_function {
    lambda_function_arn = "${aws_lambda_function.elb.arn}"
    events              = ["s3:ObjectCreated:*"]
  }
}

resource "aws_lambda_function" "elb" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-elb"
  role             = "arn:aws:iam::002067833750:role/acct-managed/altered-logs-lambda"
  handler          = "Altered.Logs::Altered.Logs.Elb.LogElbS3EventLambda::Execute"
  source_code_hash = "${base64sha256(file("../publish.zip"))}"
  runtime          = "dotnetcore2.1"
  timeout          = "59"                                                             # srsly amazon should just have a seperate first-run timeout

  tags = "${local.altered_tags}"

  environment {
    variables = "${local.altered_tags}"
  }
}
resource "aws_lambda_permission" "elb_allow_elb_log" {
  statement_id  = "${var.repo_name}-${var.branch_name}-elb-elblog"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.elb.function_name}"
  principal     = "s3.amazonaws.com"
  source_arn    = "${data.aws_s3_bucket.elb_logs.arn}"
}
