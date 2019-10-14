provider "kibana" {}

data "kibana_index" "acl" {
  filter = {
    name   = "title"
    values = ["acl-*"]
  }
}

resource "kibana_search" "builds" {
  name            = "Builds"
  display_columns = ["_source"]
  sort_by_columns = ["time"]

  search = {
    index = "${data.kibana_index.main.id}"

    filters = [
      {
        bool = {
          should = [{
            prefix = {
              "log.name.keyword" = "CodeBuild"
            }
          },
            {
              prefix = {
                "log.name.keyword" = "Docker"
              }
            },
            {
              prefix = {
                "log.name.keyword" = "Terraform"
              }
            },
            {
              prefix = {
                "log.name.keyword" = "Test"
              }
            },
          ]
        }
      },
    ]
  }
}

resource "kibana_visualization" "builds" {
  name            = "Builds"
  saved_search_id = "${kibana_search.builds.id}"

  visualization_state = <<EOF
{
    "aggs": [{
        "enabled": true,
        "id": "1",
        "params": {},
        "schema": "metric",
        "type": "count"
    }, {
        "enabled": true,
        "id": "5",
        "params": {
            "customLabel": "repo",
            "field": "repo.keyword",
            "missingBucket": false,
            "missingBucketLabel": "Missing",
            "order": "desc",
            "orderAgg": {
                "enabled": true,
                "id": "5-orderAgg",
                "params": {
                    "field": "time"
                },
                "schema": {
                    "aggFilter": ["!top_hits", "!percentiles", "!median", "!std_dev", "!derivative", "!moving_avg", "!serial_diff", "!cumulative_sum", "!avg_bucket", "!max_bucket", "!min_bucket", "!sum_bucket"],
                    "deprecate": false,
                    "editor": false,
                    "group": "none",
                    "hideCustomLabel": true,
                    "max": null,
                    "min": 0,
                    "name": "orderAgg",
                    "params": [],
                    "title": "Order Agg"
                },
                "type": "max"
            },
            "orderBy": "custom",
            "otherBucket": false,
            "otherBucketLabel": "Other",
            "size": 5
        },
        "schema": "bucket",
        "type": "terms"
    }, {
        "enabled": true,
        "id": "3",
        "params": {
            "customLabel": "build",
            "field": "log.requestId.keyword",
            "missingBucket": false,
            "missingBucketLabel": "Missing",
            "order": "desc",
            "orderAgg": {
                "enabled": true,
                "id": "3-orderAgg",
                "params": {
                    "field": "@timestamp"
                },
                "schema": {
                    "aggFilter": ["!top_hits", "!percentiles", "!median", "!std_dev", "!derivative", "!moving_avg", "!serial_diff", "!cumulative_sum", "!avg_bucket", "!max_bucket", "!min_bucket", "!sum_bucket"],
                    "deprecate": false,
                    "editor": false,
                    "group": "none",
                    "hideCustomLabel": true,
                    "max": null,
                    "min": 0,
                    "name": "orderAgg",
                    "params": [],
                    "title": "Order Agg"
                },
                "type": "max"
            },
            "orderBy": "custom",
            "otherBucket": false,
            "otherBucketLabel": "Other",
            "size": 5
        },
        "schema": "bucket",
        "type": "terms"
    }, {
        "enabled": true,
        "id": "4",
        "params": {
            "customLabel": "build-status",
            "field": "log.message.detail.build-status.keyword",
            "missingBucket": true,
            "missingBucketLabel": "-",
            "order": "desc",
            "orderAgg": {
                "enabled": true,
                "id": "4-orderAgg",
                "params": {
                    "field": "time"
                },
                "schema": {
                    "aggFilter": ["!top_hits", "!percentiles", "!median", "!std_dev", "!derivative", "!moving_avg", "!serial_diff", "!cumulative_sum", "!avg_bucket", "!max_bucket", "!min_bucket", "!sum_bucket"],
                    "deprecate": false,
                    "editor": false,
                    "group": "none",
                    "hideCustomLabel": true,
                    "max": null,
                    "min": 0,
                    "name": "orderAgg",
                    "params": [],
                    "title": "Order Agg"
                },
                "type": "max"
            },
            "orderBy": "custom",
            "otherBucket": false,
            "otherBucketLabel": "Other",
            "size": 5
        },
        "schema": "bucket",
        "type": "terms"
    }, {
        "enabled": true,
        "id": "6",
        "params": {
            "customLabel": "build-phase",
            "field": "log.message.detail.completed-phase.keyword",
            "missingBucket": true,
            "missingBucketLabel": "-",
            "order": "desc",
            "orderAgg": {
                "enabled": true,
                "id": "6-orderAgg",
                "params": {
                    "field": "time"
                },
                "schema": {
                    "aggFilter": ["!top_hits", "!percentiles", "!median", "!std_dev", "!derivative", "!moving_avg", "!serial_diff", "!cumulative_sum", "!avg_bucket", "!max_bucket", "!min_bucket", "!sum_bucket"],
                    "deprecate": false,
                    "editor": false,
                    "group": "none",
                    "hideCustomLabel": true,
                    "max": null,
                    "min": 0,
                    "name": "orderAgg",
                    "params": [],
                    "title": "Order Agg"
                },
                "type": "max"
            },
            "orderBy": "custom",
            "otherBucket": false,
            "otherBucketLabel": "-",
            "size": 5
        },
        "schema": "bucket",
        "type": "terms"
    }, {
        "enabled": true,
        "id": "7",
        "params": {
            "customLabel": "testOutcome",
            "field": "log.response.testOutcome.keyword",
            "missingBucket": true,
            "missingBucketLabel": "-",
            "order": "desc",
            "orderAgg": {
                "enabled": true,
                "id": "7-orderAgg",
                "params": {
                    "field": "time"
                },
                "schema": {
                    "aggFilter": ["!top_hits", "!percentiles", "!median", "!std_dev", "!derivative", "!moving_avg", "!serial_diff", "!cumulative_sum", "!avg_bucket", "!max_bucket", "!min_bucket", "!sum_bucket"],
                    "deprecate": false,
                    "editor": false,
                    "group": "none",
                    "hideCustomLabel": true,
                    "max": null,
                    "min": 0,
                    "name": "orderAgg",
                    "params": [],
                    "title": "Order Agg"
                },
                "type": "max"
            },
            "orderBy": "custom",
            "otherBucket": false,
            "otherBucketLabel": "Other",
            "size": 5
        },
        "schema": "bucket",
        "type": "terms"
    }],
    "params": {
        "perPage": 10,
        "showMetricsAtAllLevels": false,
        "showPartialRows": false,
        "showTotal": false,
        "sort": {
            "columnIndex": null,
            "direction": null
        },
        "totalFunc": "sum"
    },
    "title": "Builds",
    "type": "table"
}
EOF
}
