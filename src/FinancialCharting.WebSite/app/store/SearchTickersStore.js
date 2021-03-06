﻿Ext.define('HighCharts.store.SearchTickersStore', {
	extend: 'Ext.data.Store',
	model: 'HighCharts.model.Ticker',
	pageSize: 25,
	autoLoad: false,
	proxy: {
		type: "jsonp",
		pageParam: "PageNumber",
		limitParam: "PerPage",
		startParam: undefined,
		//url: 'http://localhost:1476/tickers',
		url: 'http://quandlcharting.azurewebsites.net/tickers',
		extraParams:
		{
			Query: ''
		},
		reader: {
			type: 'json',
			root: 'data.tickers',
			successProperty: 'success',
			totalProperty: 'data.total_count'
		}
	}
});