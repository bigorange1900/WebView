﻿import { cpus } from "os";
import { join } from "path";
import { RuleSetRule, RuleSetUseItem } from "webpack";

import { CacheDirectoryDefault, TsConfigDefaultFileName } from "../Plugins/Resources";
import { getCurrentDirectory } from "../Plugins/Utils";

// .ts / .tsx  files
let getTypeScriptRuleSet = (useCache: boolean, configFilePath?: string): RuleSetRule => {

    const TypeScriptRule: RuleSetRule = {
        test: /\.tsx?$/
    };

    let ruleSet: RuleSetUseItem[] = [];

    // cache-loader
    if (useCache) {
        let cacheLoaderRule: RuleSetUseItem = {
            loader: "cache-loader",
            options: {
                cacheDirectory: join(getCurrentDirectory(), CacheDirectoryDefault)
            }
        };

        ruleSet = ruleSet.concat(cacheLoaderRule);
    }

    let threadLoaderRule: RuleSetUseItem = {
        loader: "thread-loader",
        options: {
            // There should be 1 CPU available for the fork-ts-checker-webpack-plugin
            workers: cpus().length - 1
        }
    };

    let tsLoaderRule: RuleSetUseItem = {
        loader: "ts-loader",
        options: {
            happyPackMode: true,
            configFile: configFilePath || TsConfigDefaultFileName
        }
    };

    // thread-loader, ts-loader
    TypeScriptRule.use = ruleSet.concat(threadLoaderRule, tsLoaderRule);
    return TypeScriptRule;
}

export default getTypeScriptRuleSet;
