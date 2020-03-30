data "aws_s3_bucket" "tf_state" {
    bucket = "altered-terraform"
}

resource "aws_s3_bucket_notification" "tf_notify" {
  bucket = "${data.aws_s3_bucket.tf_state.id}"

  lambda_function {
    lambda_function_arn = "${aws_lambda_function.tf.arn}"
    events              = ["s3:ObjectCreated:*"]
  }
}

resource "aws_lambda_function" "tf" {
  filename         = "../publish.zip"
  function_name    = "${var.repo_name}-${var.branch_name}-tf"
  role             = "arn:aws:iam::002067833750:role/acct-managed/altered-logs-lambda"
  handler          = "Altered.Logs::Altered.Logs.Terraform.LogTerraformS3EventLambda::Execute"
  source_code_hash = "${filebase64sha256("../publish.zip")}"
  runtime          = "dotnetcore2.1"
  timeout          = "59"                                                             # srsly amazon should just have a seperate first-run timeout

  tags = "${module.tags.tags}"

  environment {
    variables = "${local.altered_tags}"
  }
}
resource "aws_lambda_permission" "tf_allow_s3" {
  statement_id  = "${var.repo_name}-${var.branch_name}-tf-s3"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.tf.function_name}"
  principal     = "s3.amazonaws.com"
  source_arn    = "${data.aws_s3_bucket.tf_state.arn}"
}
