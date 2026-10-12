# Manipulating Harp Messages

This section covers advanced message manipulation techniques that allow destructuring and constructing Harp messages at will from their essential elements. It is useful mostly for applications where routing of Harp messages is dynamic, when we need to build new commands based on previous responses, or when manipulating timestamped sequences for reactive computations that require keeping the original hardware timestamp of the result.

## Modifying message fields

We discussed how to generate [`HarpMessage`](xref:Bonsai.Harp.HarpMessage) objects by using either the [`CreateMessage`](xref:Bonsai.Harp.CreateMessage) operator or by using [`Format`](xref:Bonsai.Harp.Format) with compatible data types. However, in specific cases, we might want to take a given `HarpMessage` and simply change the value of one or more of its metadata fields, for example its `Address`.

We could, in theory, [`Parse`](xref:Bonsai.Harp.Parse) the incoming message into its components and "reassemble" everything with one of the constructor operators, but in practice this tends to be too cumbersome and inefficient.

Another option is to use a specific overload of the [`Format`](xref:Bonsai.Harp.Format) operator that accepts a sequence of `HarpMessage` objects as input. In this case we can replace the value of one or more of its metadata fields by specifying non-null values in the operator properties.

For instance, to change the `Address` of a `HarpMessage`:

:::workflow
![ModifyHarpMessageAddress](~/workflows/modify-message-address.bonsai)
:::

> [!Note]
> These overloads are only available when the `Register` property of the [`Parse`](xref:Bonsai.Harp.Format) operator is set to [`FormatMessagePayload`](xref:Bonsai.Harp.FormatMessagePayload).

## Injecting or modifying message timestamps

The [`Format`](xref:Bonsai.Harp.Format) operator also allows us to inject or modify a timestamp of a `HarpMessage` object. This is done by providing an input of type [`Timestamped<T>`](xref:Bonsai.Harp.Timestamped`1), where `T` in this case would be of type `HarpMessage`. For instance, to set the timestamp of a given message to `0`:

:::workflow
![ModifyHarpMessageTimestampFromDouble](~/workflows/modify-message-double-timestamp.bonsai)
:::

Alternatively, it is also possible to construct the [`Timestamped<T>`](xref:Bonsai.Harp.Timestamped`1) object from another message. In this case, the temporal information will be extracted from the message in the second input. This is most useful when we need to create a timestamped value from a message that already contains a timestamp, for instance, from a Harp Device:

:::workflow
![ModifyHarpMessageTimestamp](~/workflows/modify-message-timestamp.bonsai)
:::

## Accessing the message timestamp

The `Payload` portion of a Harp message can contain information about the hardware timestamp of the message as given by the device. To access this information, we can use the [`Parse`](xref:Bonsai.Harp.Parse) operator as discussed previously. In its simplest form, the core [`Parse`](xref:Bonsai.Harp.Parse) operator can, for any `HarpMessage`, extract the timestamp information by setting the [`PayloadType`](xref:Bonsai.Harp.ParseMessagePayload.PayloadType) property to `Timestamp`.

:::workflow
![ParseTimestamp](~/workflows/parse-timestamp.bonsai)
:::

It is important to keep in mind that if a `HarpMessage` does not contain the optional `Timestamp` field, the `Parse` operator will throw an error. We can check if a message has a valid `Timestamp` field by using the [`IsTimestamped`](xref:Bonsai.Harp.HarpMessage.IsTimestamped) property of the `HarpMessage` class:

:::workflow
![ParseTimestamp](~/workflows/filter-timestamped.bonsai)
:::

> [!Warning]
> When parsing the raw message timestamp, the `Parse` operator will not parse the `Payload` data portion of the message. This technique is useful when we are only interested in the `Timestamp` portion of the message.

If we are interested in simultaneously parsing the data *and* timestamp information of a message, for specific registers, the alternative is to select the appropriate `Timestamped<Register>` value from the [`Register`](xref:Bonsai.Harp.ParseBuilder.Register) property. This pattern is available for both the core `Bonsai.Harp` package and device-specific packages.

:::workflow
![ParseTimestampData](~/workflows/parse-timestamp-data.bonsai)
:::

## Timestamping generic data

The timestamp provided by each `HarpMessage` is often the result of a device operation and will thus be assigned in hardware. However, it can be very useful to assign hardware timestamps to events that occur in software, on the host side.

For example, we may want to know when a mouse button was pressed in a somewhat meaningful temporal reference relative to the acquired Harp data. Unfortunately, the host PC does not run the same clock synchronization protocol as Harp devices, and we cannot use this clock to timestamp events. However, we can still assign a timestamp to any host event by simply using the timestamp from the latest available message emitted by the board.

Since the round-trip delay between host and device is typically small and with low jitter (less than 5 ms) we can use this strategy to timestamp software events. Furthermore, the result of [`WithLatestFrom`](xref:Bonsai.Reactive.WithLatestFrom) can be readily converted into a [`Timestamped<T>`](xref:Bonsai.Harp.Timestamped`1) value using the [`CreateTimestamped`](xref:Bonsai.Harp.CreateTimestamped) operator, as shown below.

:::workflow
![WithLatestTimestampFiltered](~/workflows/withlatest-timestamp-filtered.bonsai)
:::

> [!Note]
> Harp devices runs a real-time operating system (RTOS) where event timestamping takes a high priority. The host PC, on the other hand, runs a non-RTOS operating system where event timestamping is not a priority. This strategy is not ideal for high-precision timing applications. Furthermore, in some systems it is possible to observe a larger than expected jitter, so we always recommend benchmarking all timings before using this technique.

> [!Warning]
> This strategy rests on the assumption that the host has access to a steady stream of messages from the device. However, while some devices provide high-frequency events (e.g. 1 kHz analog read events from a Behavior board), many other boards can be typically silent. In these cases, the temporal stream will be non-homogenous and with poor resolution, and users should use an alternative strategy.

An example of an alternative strategy is to request a `Read` operation from the board for each software event to be timestamped. Since each `Read` message is also timestamped, this can be assigned to the software event using the following pattern of asynchronous "request-timestamp" strategy.

:::workflow
![AsyncRequestTimestamp](~/workflows/timestamp-async.bonsai)
:::

Finally, we can also timestamp values from an arbitrary source of seconds, for example from an offline file or from a constant declared in the workflow. This is possible due to specific overloads of the [`CreateTimestamped`](xref:Bonsai.Harp.CreateTimestamped) operator that accept `double` values as timestamps.

:::workflow
![CreateTimestamped](~/workflows/create-timestamped.bonsai)
:::

## Preserving timestamps in processing pipelines

When consuming timestamped messages arriving from a Harp device, we may need to compute some transformation on the incoming data (say a value from the ADC that we might want to convert or scale) while maintaining the original timestamp of the data that gave rise to it.

The [`ConvertTimestamped`](xref:Bonsai.Harp.ConvertTimestamped) operator makes this process easier. This operator takes as input a [`Timestamped<T>`](xref:Bonsai.Harp.Timestamped`1) value, e.g. the result of a [`Parse`](xref:Bonsai.Harp.Parse) operator and allows us to define any necessary conversion logic inside the operator.

> [!Warning]
> All operators inside the nested `ConvertTimestamped` operator should be synchronous in nature to ensure that all timestamps are correctly paired with the input data. In other words, the nested operation should behave as a [`Transform`](https://bonsai-rx.org/docs/articles/operators.html) operator.

For example, to manipulate the value of a register, and reassign its original timestamp:

:::workflow
![ConvertTimestamped](~/workflows/convert-timestamped.bonsai)
:::

## Creating messages with a timestamped payload

So far, we have only covered cases where the timestamp is extracted from an existing `HarpMessage`. However, in certain cases it might also be useful to create a `HarpMessage` with a `Timestamp` in the `Payload` field. Mirroring its use to generate `HarpMessages`, [`CreateMessage`](xref:Bonsai.Harp.CreateMessage) can also be used to *inject* a timestamped. We achieve this by:

1. Setting the `PayloadType` (or `Payload`) property to a `Timestamped` type (e.g. `TimestampedU8`);
2. Passing a sequence of [`Timestamped<T>`](xref:Bonsai.Harp.Timestamped`1) values to the operator.

:::workflow
![CreateMessageTimestamped](~/workflows/create-message-timestamped.bonsai)
:::

Similarly, we can use [`Format`](xref:Bonsai.Harp.Format) to simultaneously inject a timestamp in the message payload. To do this we can use the same pattern as above for [`CreateMessage`](xref:Bonsai.Harp.CreateMessage), but this time we need to make sure that the `T` type is compatible with the selected [`PayloadType`](xref:Bonsai.Harp.FormatMessagePayload.PayloadType) or [`Register`](xref:Bonsai.Harp.FormatBuilder.Register) properties in the `Format` operator.

:::workflow
![FormatTimestamped](~/workflows/format-timestamped.bonsai)
:::

