resource "aws_iam_role" "iam" {
    name = "${var.repo_name}-${var.branch_name}-iam"
    tags = "${local.altered_tags}"

    assume_role_policy = "${data.aws_iam_policy_document.iam_assume_role_policy.json}"
}

data "aws_iam_policy_document" "iam" {
    statement {
        sid       = "get"
        effect    = "Allow"
        resources = [
            "${data.aws_s3_bucket.alb_logs.arn}/*",
            "${data.aws_s3_bucket.elb_logs.arn}/*",
            "${data.aws_s3_bucket.tf_state.arn}/*"
        ]
        actions = [
            "s3:GetObject"
        ]
    }

    statement {
        sid = "log"
        effect = "Allow"
        resources = [
            "*"
        ]
        actions = [
            # create stream, put event, put metric
            "logs:*",
            "cloudwatch:*"
        ]
    }

    statement {
        sid = "codebuild"
        effect = "Allow"
        resources = [
            "*"
        ]
        actions = [
            "codebuild:BatchGetProjects"
        ]
    }
}

data "aws_iam_policy_document" "iam_assume_role_policy" {
    statement {
        sid       = "exec"
        effect    = "Allow"
        actions = [
            "sts:AssumeRole"
        ]

        principals {
            type = "Service"
            # everything
            identifiers = ["lambda.amazonaws.com", "ec2.amazonaws.com", "ecs-tasks.amazonaws.com", "ecs.amazonaws.com", "ec2.application-autoscaling.amazonaws.com", "ecs.application-autoscaling.amazonaws.com", "es.amazonaws.com"]
        }
    }
}

resource "aws_iam_policy" "iam" {
  name   = "${var.repo_name}-${var.branch_name}-alb"
  policy = "${data.aws_iam_policy_document.iam.json}"
}

resource "aws_iam_role_policy_attachment" "iam" {
  role       = "${aws_iam_role.iam.name}"
  policy_arn = "${aws_iam_policy.iam.arn}"
}

resource "aws_iam_role_policy_attachment" "iam_AmazonECSTaskExecutionRolePolicy" {
  role       = "${aws_iam_role.iam.name}"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy_attachment" "iam_AmazonEC2ContainerServiceforEC2Role" {
  role       = "${aws_iam_role.iam.name}"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}
