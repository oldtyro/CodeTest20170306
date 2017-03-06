/// <reference path="angular.js" />

var myApp = angular
    .module("merchantModule", [])
    .controller("merchantController", function ($scope, $http) {
        $scope.page_size = 10;
        $scope.page_number = 1;
        $scope.page_sizes = [5, 10, 20, 50, 100];
        $scope.hide = false;

        var successCallBack = function (response) {
            $scope.code = response.data["response_code"];

            if ($scope.code == 0)
            {
                $scope.merchants = response.data["data"];
                $scope.page_size = response.data["pagination"].page_size;
                $scope.page_number = response.data["pagination"].page_number;
                $scope.total_records = response.data["pagination"].total_records;
                $scope.total_pages = response.data["pagination"].total_pages;
                $scope.page_numbers = setPageNumbers($scope.total_pages);
            }
            else
            {
                $scope.error = response.data["data"][0].message;
                $scope.hide = true;
            }
        }

        var errorCallBack = function (response) {
            $scope.error = response.data;
            $scope.hide = true;
        }

        $http({
            method: 'GET',
            url: 'http://api.demo.muulla.com/cms/merchant/all/active/' + $scope.page_size + '/' + $scope.page_number,
            headers: { 'Authorization': 'Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI1NGQxOTY4MGI1MWMxNTI2MGI5NDRmZDUiLCJpc3N1ZV9kYXRlIjoiMjAxNS0wOS0wOVQwNToxMzo1My40NThaIn0.Hk2XypA_KMUnIKdSVYnwq3Rn3QyMNSQ-e80-sZsA9bY' }
        })
            .then(successCallBack, errorCallBack);

        var loadData = function() {
            $http({
                method: 'GET',
                url: 'http://api.demo.muulla.com/cms/merchant/all/active/' + $scope.page_size + '/' + $scope.page_number,
                headers: { 'Authorization': 'Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI1NGQxOTY4MGI1MWMxNTI2MGI5NDRmZDUiLCJpc3N1ZV9kYXRlIjoiMjAxNS0wOS0wOVQwNToxMzo1My40NThaIn0.Hk2XypA_KMUnIKdSVYnwq3Rn3QyMNSQ-e80-sZsA9bY' }
            })
            .then(successCallBack, errorCallBack);
        }

        var setPageNumbers = function(totalPages) {
            var pageNumbers = new Array();
            for (i = 0; i < totalPages; i++)
            {
                pageNumbers[i] = i + 1;
            }
            return pageNumbers;
        }

        $scope.reloadDataAfterChangeSize = function()
        {
            $scope.page_number = 1;
            loadData();
        }

        $scope.reloadDataAfterChangeNumber = function () {
            loadData();
        }
    })
    .controller("merchantMVCController", function ($scope, $http) {
    });