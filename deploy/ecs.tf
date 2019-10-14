resource "aws_cloudwatch_log_group" "container_logs" {
  name = "${var.repo_name}-${var.branch_name}"
  tags = "${module.tags.tags}"
}

module "container_definition" {
  source          = "git::https://github.com/cloudposse/terraform-aws-ecs-container-definition.git?ref=0.7.0"
  container_name  = "${var.repo_name}-${var.branch_name}"
  container_image = "${var.ecr_repo}:${var.repo_name}-${var.branch_name}-${var.source_rev}"

  port_mappings = [
    {
      containerPort = 80
      protocol      = "tcp"
    },
    {
      containerPort = 443
      protocol      = "tcp"
    },
  ]

  log_options = {
    "awslogs-group"         = "${aws_cloudwatch_log_group.container_logs.name}"
    "awslogs-region"        = "${var.aws_region}"
    "awslogs-stream-prefix" = "ecs"
  }

  environment = [
    {
      name  = "app"
      value = "${var.repo_name}"
    },
    {
      name  = "env"
      value = "${var.branch_name}"
    },
    {
      name  = "sha"
      value = "${var.source_rev}"
    },
    {
      name = "elasticsearch_domain"
      value = "https://${aws_elasticsearch_domain.search.endpoint}"
    }
  ]
}

resource "aws_ecs_cluster" "cluster" {
  name = "${var.repo_name}-${var.branch_name}"
}

resource "aws_ecs_task_definition" "default" {
  family                   = "${module.tags.id}"
  container_definitions    = "${module.container_definition.json}"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = "256"
  memory                   = "512"
  execution_role_arn       = "arn:aws:iam::002067833750:role/acct-managed/altered-api"
  task_role_arn            = "arn:aws:iam::002067833750:role/acct-managed/altered-api"
  tags                     = "${module.tags.tags}"
}

resource "aws_security_group_rule" "allow_all_egress" {
  type              = "egress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.security_group.id}"
}

resource "aws_security_group_rule" "allow_http_ingress" {
  type              = "ingress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.security_group.id}"
}

resource "aws_security_group_rule" "allow_https_ingress" {
  type              = "ingress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.security_group.id}"
}

resource "aws_ecs_service" "default" {
  name            = "${module.tags.id}"
  task_definition = "${aws_ecs_task_definition.default.family}:${aws_ecs_task_definition.default.revision}"

  desired_count = 1

  launch_type = "FARGATE"

  cluster = "${aws_ecs_cluster.cluster.arn}"

  #enable_ecs_managed_tags = true
  tags           = "${merge(module.tags.tags, local.altered_tags)}"
  propagate_tags = "SERVICE"

  network_configuration {
    security_groups  = ["${aws_security_group.security_group.id}"]
    subnets          = "${data.aws_subnet_ids.subnets.ids}"
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = "${aws_lb_target_group.ecs.arn}"
    container_name   = "${var.repo_name}-${var.branch_name}"
    container_port   = "80"
  }

  lifecycle {
    ignore_changes = ["desired_count"]
  }

  # workaround for https://github.com/hashicorp/terraform/issues/12634
  depends_on = [
    "aws_lb_listener_rule.ecs_https",
  ]
}

resource "aws_lb_target_group" "ecs" {
  name        = "${var.repo_name}-${var.branch_name}-ecs"
  port        = 80
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = "${data.aws_vpc.vpc.id}"

  tags = "${module.tags.tags}"
}

resource "aws_lb_listener_rule" "ecs_https" {
  listener_arn = "${aws_lb_listener.https.arn}"

  action {
    type             = "forward"
    target_group_arn = "${aws_lb_target_group.ecs.arn}"
  }

  condition {
    field  = "path-pattern"
    values = ["/*"]
  }
}
